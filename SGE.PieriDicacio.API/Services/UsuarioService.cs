using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Usuario;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<UsuarioResponseDto>>> ObtenerTodosAsync()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.Nombres)
                .Select(u => new UsuarioResponseDto
                {
                    Id = u.Id,
                    IdRol = u.IdRol,
                    Rol = u.Rol.Nombre,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Username = u.Username,
                    Correo = u.Correo,
                    Telefono = u.Telefono,
                    Estado = u.Estado
                })
                .ToListAsync();

            return Result<List<UsuarioResponseDto>>.Ok(usuarios);
        }

        public async Task<Result<UsuarioResponseDto>> ObtenerPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Id == id)
                .Select(u => new UsuarioResponseDto
                {
                    Id = u.Id,
                    IdRol = u.IdRol,
                    Rol = u.Rol.Nombre,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Username = u.Username,
                    Correo = u.Correo,
                    Telefono = u.Telefono,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();

            return usuario != null
                ? Result<UsuarioResponseDto>.Ok(usuario)
                : Result<UsuarioResponseDto>.Fail("Usuario no encontrado");
        }

        public async Task<Result<UsuarioResponseDto>> CrearAsync(UsuarioCreateDto dto)
        {
            var rolExiste = await _context.Roles.AnyAsync(r => r.Id == dto.IdRol && r.Estado);
            if (!rolExiste)
                return Result<UsuarioResponseDto>.Fail("El rol seleccionado no existe o está desactivado");

            var usernameExiste = await _context.Usuarios
                .AnyAsync(u => u.Username.ToLower() == dto.Username.Trim().ToLower());

            if (usernameExiste)
                return Result<UsuarioResponseDto>.Fail("El nombre de usuario ya está en uso");

            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo.ToLower() == dto.Correo.Trim().ToLower());

            if (correoExiste)
                return Result<UsuarioResponseDto>.Fail("El correo ya está registrado");

            var usuario = new Usuario
            {
                IdRol = dto.IdRol,
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Username = dto.Username.Trim(),
                Correo = dto.Correo.Trim(),
                Telefono = dto.Telefono?.Trim(),
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(usuario.Id, "Crear", $"Se creó el usuario {usuario.Username}");

            var rol = await _context.Roles.FindAsync(usuario.IdRol);

            return Result<UsuarioResponseDto>.Ok(new UsuarioResponseDto
            {
                Id = usuario.Id,
                IdRol = usuario.IdRol,
                Rol = rol?.Nombre ?? string.Empty,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Username = usuario.Username,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Estado = usuario.Estado
            });
        }

        public async Task<Result<UsuarioResponseDto>> ActualizarAsync(int id, UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return Result<UsuarioResponseDto>.Fail("Usuario no encontrado");

            var rolExiste = await _context.Roles.AnyAsync(r => r.Id == dto.IdRol && r.Estado);
            if (!rolExiste)
                return Result<UsuarioResponseDto>.Fail("El rol seleccionado no existe o está desactivado");

            var usernameExiste = await _context.Usuarios
                .AnyAsync(u => u.Id != id && u.Username.ToLower() == dto.Username.Trim().ToLower());

            if (usernameExiste)
                return Result<UsuarioResponseDto>.Fail("El nombre de usuario ya está en uso");

            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Id != id && u.Correo.ToLower() == dto.Correo.Trim().ToLower());

            if (correoExiste)
                return Result<UsuarioResponseDto>.Fail("El correo ya está registrado");

            usuario.IdRol = dto.IdRol;
            usuario.Nombres = dto.Nombres.Trim();
            usuario.Apellidos = dto.Apellidos.Trim();
            usuario.Username = dto.Username.Trim();
            usuario.Correo = dto.Correo.Trim();
            usuario.Telefono = dto.Telefono?.Trim();
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(usuario.Id, "Actualizar", $"Se actualizó el usuario {usuario.Username}");

            var rol = await _context.Roles.FindAsync(usuario.IdRol);

            return Result<UsuarioResponseDto>.Ok(new UsuarioResponseDto
            {
                Id = usuario.Id,
                IdRol = usuario.IdRol,
                Rol = rol?.Nombre ?? string.Empty,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Username = usuario.Username,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Estado = usuario.Estado
            });
        }

        public async Task<Result<bool>> CambiarEstadoAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return Result<bool>.Fail("Usuario no encontrado");

            usuario.Estado = !usuario.Estado;
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(usuario.Id, "Cambiar estado", $"Se {(usuario.Estado ? "activó" : "desactivó")} el usuario {usuario.Username}");

            return Result<bool>.Ok(true);
        }

        private async Task RegistrarBitacoraAsync(int idRegistro, string accion, string descripcion)
        {
            try
            {
                var idUsuario = _httpContextAccessor.HttpContext?.GetUserIdFromHttpContext() ?? 0;
                if (idUsuario == 0)
                    return;

                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = "Usuarios",
                    Accion = accion,
                    TablaAfectada = "usuario",
                    IdRegistroAfectado = idRegistro,
                    Descripcion = descripcion,
                    IpEquipo = AuditHelper.ObtenerIp(_httpContextAccessor.HttpContext)
                }, idUsuario);
            }
            catch
            {
                // La bitácora nunca debe impedir la operación principal
            }
        }
    }
}
