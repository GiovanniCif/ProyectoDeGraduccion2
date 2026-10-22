using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Permiso;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermisoService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<PermisoResponseDto>>> ObtenerPermisosAsync()
        {
            var permisos = await _context.Permisos
                .Where(p => p.Estado)
                .OrderBy(p => p.Modulo)
                .ThenBy(p => p.Nombre)
                .Select(p => new PermisoResponseDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Modulo = p.Modulo,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<PermisoResponseDto>>.Ok(permisos);
        }

        public async Task<Result<List<PermisoResponseDto>>> ObtenerPermisosPorRolAsync(int rolId)
        {
            var rolExiste = await _context.Roles.AnyAsync(r => r.Id == rolId);
            if (!rolExiste)
                return Result<List<PermisoResponseDto>>.Fail("Rol no encontrado");

            var permisos = await _context.RolesPermisos
                .Where(rp => rp.IdRol == rolId)
                .OrderBy(rp => rp.Permiso.Modulo)
                .ThenBy(rp => rp.Permiso.Nombre)
                .Select(rp => new PermisoResponseDto
                {
                    Id = rp.Permiso.Id,
                    Nombre = rp.Permiso.Nombre,
                    Modulo = rp.Permiso.Modulo,
                    Descripcion = rp.Permiso.Descripcion,
                    Estado = rp.Permiso.Estado
                })
                .ToListAsync();

            return Result<List<PermisoResponseDto>>.Ok(permisos);
        }

        public async Task<Result<bool>> AsignarPermisoAsync(int rolId, AsignarPermisoRolDto dto)
        {
            var rol = await _context.Roles.FindAsync(rolId);
            if (rol == null)
                return Result<bool>.Fail("Rol no encontrado");

            if (string.Equals(rol.Nombre, "Administrador", StringComparison.OrdinalIgnoreCase))
                return Result<bool>.Fail("El rol Administrador no se puede modificar");

            var permiso = await _context.Permisos.FindAsync(dto.IdPermiso);
            if (permiso == null)
                return Result<bool>.Fail("El permiso no existe");

            var yaAsignado = await _context.RolesPermisos
                .AnyAsync(rp => rp.IdRol == rolId && rp.IdPermiso == dto.IdPermiso);

            if (yaAsignado)
                return Result<bool>.Fail("El permiso ya está asignado a este rol");

            _context.RolesPermisos.Add(new RolPermiso
            {
                IdRol = rolId,
                IdPermiso = dto.IdPermiso,
                FechaCreacion = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync($"Se asignó el permiso \"{permiso.Nombre}\" al rol {rol.Nombre}", rolId);

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> QuitarPermisoAsync(int rolId, int idPermiso)
        {
            var rol = await _context.Roles.FindAsync(rolId);
            if (rol == null)
                return Result<bool>.Fail("Rol no encontrado");

            if (string.Equals(rol.Nombre, "Administrador", StringComparison.OrdinalIgnoreCase))
                return Result<bool>.Fail("El rol Administrador no se puede modificar");

            var permiso = await _context.Permisos.FindAsync(idPermiso);

            var vinculo = await _context.RolesPermisos
                .FirstOrDefaultAsync(rp => rp.IdRol == rolId && rp.IdPermiso == idPermiso);

            if (vinculo == null)
                return Result<bool>.Fail("El permiso no está asignado a este rol");

            _context.RolesPermisos.Remove(vinculo);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync($"Se quitó el permiso \"{permiso?.Nombre ?? "desconocido"}\" del rol {rol.Nombre}", rolId);

            return Result<bool>.Ok(true);
        }

        private async Task RegistrarBitacoraAsync(string descripcion, int idRol)
        {
            try
            {
                var idUsuario = _httpContextAccessor.HttpContext?.GetUserIdFromHttpContext() ?? 0;
                if (idUsuario == 0)
                    return;

                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = "Roles",
                    Accion = "Permisos",
                    TablaAfectada = "rol_permiso",
                    IdRegistroAfectado = idRol,
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
