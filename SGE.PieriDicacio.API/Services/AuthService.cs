using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Auth;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                    .ThenInclude(r => r.RolesPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(u =>
                    (u.Username == request.UsernameOrCorreo || u.Correo == request.UsernameOrCorreo)
                    && u.Estado
                    && u.Rol.Estado);

            if (usuario == null)
                return null;

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > DateTime.UtcNow)
                return null;

            if (!PasswordHelper.VerifyPassword(request.Password, usuario.PasswordHash))
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= 5)
                {
                    usuario.BloqueadoHasta = DateTime.UtcNow.AddMinutes(15);
                }
                await _context.SaveChangesAsync();

                await RegistrarBitacoraAsync(usuario, "Inicio de sesión fallido", $"Intento de inicio de sesión fallido para {usuario.Username}");
                return null;
            }

            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;
            usuario.UltimoAcceso = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);
            var expiration = DateTime.UtcNow.AddMinutes(expireMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            foreach (var permiso in usuario.Rol.RolesPermisos.Select(rp => rp.Permiso.Nombre))
            {
                claims.Add(new Claim(PermissionRequirement.ClaimType, permiso));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            await RegistrarBitacoraAsync(usuario, "Inicio de sesión", $"Inicio de sesión exitoso para {usuario.Username}");

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiration,
                Username = usuario.Username,
                NombreCompleto = $"{usuario.Nombres} {usuario.Apellidos}",
                Rol = usuario.Rol.Nombre
            };
        }

        private async Task RegistrarBitacoraAsync(Entities.Usuario usuario, string accion, string descripcion)
        {
            try
            {
                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = "Autenticación",
                    Accion = accion,
                    TablaAfectada = "usuario",
                    IdRegistroAfectado = usuario.Id,
                    Descripcion = descripcion,
                    IpEquipo = AuditHelper.ObtenerIp(_httpContextAccessor.HttpContext)
                }, usuario.Id);
            }
            catch
            {
                // La bitácora nunca debe impedir el inicio de sesión
            }
        }
    }
}
