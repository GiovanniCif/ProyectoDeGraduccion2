using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Rol;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class RolService : IRolService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RolService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<RolResponseDto>>> ObtenerTodosAsync()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.Nombre)
                .Select(r => new RolResponseDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado
                })
                .ToListAsync();

            return Result<List<RolResponseDto>>.Ok(roles);
        }

        public async Task<Result<RolResponseDto>> ObtenerPorIdAsync(int id)
        {
            var rol = await _context.Roles
                .Where(r => r.Id == id)
                .Select(r => new RolResponseDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado
                })
                .FirstOrDefaultAsync();

            return rol != null
                ? Result<RolResponseDto>.Ok(rol)
                : Result<RolResponseDto>.Fail("Rol no encontrado");
        }

        public async Task<Result<RolResponseDto>> CrearAsync(RolCreateDto dto)
        {
            var existe = await _context.Roles
                .AnyAsync(r => r.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

            if (existe)
                return Result<RolResponseDto>.Fail("Ya existe un rol con ese nombre");

            var rol = new Rol
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(rol.Id, "Crear", $"Se creó el rol {rol.Nombre}");

            return Result<RolResponseDto>.Ok(new RolResponseDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Estado = rol.Estado
            });
        }

        public async Task<Result<RolResponseDto>> ActualizarAsync(int id, RolUpdateDto dto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return Result<RolResponseDto>.Fail("Rol no encontrado");

            if (string.Equals(rol.Nombre, "Administrador", StringComparison.OrdinalIgnoreCase))
                return Result<RolResponseDto>.Fail("El rol Administrador no se puede modificar");

            var existe = await _context.Roles
                .AnyAsync(r => r.Id != id && r.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

            if (existe)
                return Result<RolResponseDto>.Fail("Ya existe un rol con ese nombre");

            rol.Nombre = dto.Nombre.Trim();
            rol.Descripcion = dto.Descripcion?.Trim();
            rol.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(rol.Id, "Actualizar", $"Se actualizó el rol {rol.Nombre}");

            return Result<RolResponseDto>.Ok(new RolResponseDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Estado = rol.Estado
            });
        }

        public async Task<Result<bool>> CambiarEstadoAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return Result<bool>.Fail("Rol no encontrado");

            if (string.Equals(rol.Nombre, "Administrador", StringComparison.OrdinalIgnoreCase))
                return Result<bool>.Fail("El rol Administrador no se puede desactivar");

            rol.Estado = !rol.Estado;
            rol.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(rol.Id, "Cambiar estado", $"Se {(rol.Estado ? "activó" : "desactivó")} el rol {rol.Nombre}");

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
                    Modulo = "Roles",
                    Accion = accion,
                    TablaAfectada = "rol",
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
