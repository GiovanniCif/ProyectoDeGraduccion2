using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Empleado;
using SGE.PieriDicacio.API.DTOs.Reporte;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<EmpleadoResponseDto>>> ObtenerEmpleadosAsync()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Departamento)
                .OrderBy(e => e.Nombres)
                .Select(e => new EmpleadoResponseDto
                {
                    Id = e.Id,
                    IdDepartamento = e.IdDepartamento,
                    Departamento = e.Departamento.Nombre,
                    Nombres = e.Nombres,
                    Apellidos = e.Apellidos,
                    Dpi = e.Dpi,
                    Telefono = e.Telefono,
                    Correo = e.Correo,
                    Direccion = e.Direccion,
                    Puesto = e.Puesto,
                    FechaIngreso = e.FechaIngreso,
                    Estado = e.Estado
                })
                .ToListAsync();

            return Result<List<EmpleadoResponseDto>>.Ok(empleados);
        }

        public async Task<Result<EmpleadoResponseDto>> ObtenerEmpleadoPorIdAsync(int id)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Departamento)
                .Where(e => e.Id == id)
                .Select(e => new EmpleadoResponseDto
                {
                    Id = e.Id,
                    IdDepartamento = e.IdDepartamento,
                    Departamento = e.Departamento.Nombre,
                    Nombres = e.Nombres,
                    Apellidos = e.Apellidos,
                    Dpi = e.Dpi,
                    Telefono = e.Telefono,
                    Correo = e.Correo,
                    Direccion = e.Direccion,
                    Puesto = e.Puesto,
                    FechaIngreso = e.FechaIngreso,
                    Estado = e.Estado
                })
                .FirstOrDefaultAsync();

            return empleado != null
                ? Result<EmpleadoResponseDto>.Ok(empleado)
                : Result<EmpleadoResponseDto>.Fail("Empleado no encontrado");
        }

        public async Task<Result<EmpleadoResponseDto>> CrearEmpleadoAsync(EmpleadoCreateDto dto)
        {
            var departamentoExiste = await _context.Departamentos
                .AnyAsync(d => d.Id == dto.IdDepartamento && d.Estado);

            if (!departamentoExiste)
                return Result<EmpleadoResponseDto>.Fail("El departamento seleccionado no existe o está desactivado");

            var empleado = new Empleado
            {
                IdDepartamento = dto.IdDepartamento,
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Dpi = dto.Dpi?.Trim(),
                Telefono = dto.Telefono?.Trim(),
                Correo = dto.Correo?.Trim(),
                Direccion = dto.Direccion?.Trim(),
                Puesto = dto.Puesto?.Trim(),
                FechaIngreso = dto.FechaIngreso,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return await ObtenerEmpleadoPorIdAsync(empleado.Id);
        }

        public async Task<Result<EmpleadoResponseDto>> ActualizarEmpleadoAsync(int id, EmpleadoUpdateDto dto)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return Result<EmpleadoResponseDto>.Fail("Empleado no encontrado");

            var departamentoExiste = await _context.Departamentos
                .AnyAsync(d => d.Id == dto.IdDepartamento && d.Estado);

            if (!departamentoExiste)
                return Result<EmpleadoResponseDto>.Fail("El departamento seleccionado no existe o está desactivado");

            empleado.IdDepartamento = dto.IdDepartamento;
            empleado.Nombres = dto.Nombres.Trim();
            empleado.Apellidos = dto.Apellidos.Trim();
            empleado.Dpi = dto.Dpi?.Trim();
            empleado.Telefono = dto.Telefono?.Trim();
            empleado.Correo = dto.Correo?.Trim();
            empleado.Direccion = dto.Direccion?.Trim();
            empleado.Puesto = dto.Puesto?.Trim();
            empleado.FechaIngreso = dto.FechaIngreso;
            empleado.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await ObtenerEmpleadoPorIdAsync(empleado.Id);
        }

        public async Task<Result<bool>> CambiarEstadoEmpleadoAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return Result<bool>.Fail("Empleado no encontrado");

            empleado.Estado = !empleado.Estado;
            empleado.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result<bool>.Ok(true);
        }

        public async Task<Result<List<AsistenciaResponseDto>>> ObtenerAsistenciasAsync()
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Empleado)
                .OrderByDescending(a => a.Fecha)
                .Select(a => new AsistenciaResponseDto
                {
                    Id = a.Id,
                    IdEmpleado = a.IdEmpleado,
                    Empleado = a.Empleado.Nombres + " " + a.Empleado.Apellidos,
                    Fecha = a.Fecha,
                    HoraEntrada = a.HoraEntrada,
                    HoraSalida = a.HoraSalida,
                    TardanzaMinutos = a.TardanzaMinutos,
                    HorasTrabajadas = a.HorasTrabajadas,
                    Observaciones = a.Observaciones,
                    IdUsuarioRegistro = a.IdUsuarioRegistro
                })
                .ToListAsync();

            return Result<List<AsistenciaResponseDto>>.Ok(asistencias);
        }

        public async Task<Result<AsistenciaResponseDto>> RegistrarAsistenciaAsync(AsistenciaCreateDto dto, int idUsuarioRegistro)
        {
            var empleadoExiste = await _context.Empleados.AnyAsync(e => e.Id == dto.IdEmpleado && e.Estado);
            if (!empleadoExiste)
                return Result<AsistenciaResponseDto>.Fail("El empleado no existe o está desactivado");

            var yaExiste = await _context.Asistencias
                .AnyAsync(a => a.IdEmpleado == dto.IdEmpleado && a.Fecha.Date == dto.Fecha.Date);

            if (yaExiste)
                return Result<AsistenciaResponseDto>.Fail("Ya existe un registro de asistencia para este empleado en esta fecha");

            int tardanza = 0;
            decimal horasTrabajadas = 0;

            if (dto.HoraEntrada.HasValue)
            {
                var horaEsperada = new TimeSpan(8, 0, 0);
                if (dto.HoraEntrada.Value > horaEsperada)
                    tardanza = (int)(dto.HoraEntrada.Value - horaEsperada).TotalMinutes;
            }

            if (dto.HoraEntrada.HasValue && dto.HoraSalida.HasValue && dto.HoraSalida > dto.HoraEntrada)
            {
                horasTrabajadas = (decimal)(dto.HoraSalida.Value - dto.HoraEntrada.Value).TotalHours;
            }

            var asistencia = new Asistencia
            {
                IdEmpleado = dto.IdEmpleado,
                Fecha = dto.Fecha.Date,
                HoraEntrada = dto.HoraEntrada,
                HoraSalida = dto.HoraSalida,
                TardanzaMinutos = tardanza,
                HorasTrabajadas = horasTrabajadas,
                Observaciones = dto.Observaciones?.Trim(),
                IdUsuarioRegistro = idUsuarioRegistro,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            var asist = await _context.Asistencias
                .Include(a => a.Empleado)
                .Where(a => a.Id == asistencia.Id)
                .Select(a => new AsistenciaResponseDto
                {
                    Id = a.Id,
                    IdEmpleado = a.IdEmpleado,
                    Empleado = a.Empleado.Nombres + " " + a.Empleado.Apellidos,
                    Fecha = a.Fecha,
                    HoraEntrada = a.HoraEntrada,
                    HoraSalida = a.HoraSalida,
                    TardanzaMinutos = a.TardanzaMinutos,
                    HorasTrabajadas = a.HorasTrabajadas,
                    Observaciones = a.Observaciones,
                    IdUsuarioRegistro = a.IdUsuarioRegistro
                })
                .FirstOrDefaultAsync();

            return asist != null
                ? Result<AsistenciaResponseDto>.Ok(asist)
                : Result<AsistenciaResponseDto>.Fail("No se pudo registrar la asistencia");
        }

        public async Task<Result<List<AsistenciaResumenDto>>> ObtenerResumenAsistenciasAsync(int anio, int mes)
        {
            if (mes < 1 || mes > 12)
                return Result<List<AsistenciaResumenDto>>.Fail("El mes debe estar entre 1 y 12");

            var primerDia = new DateTime(anio, mes, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            var diasHabiles = 0;
            for (var d = primerDia; d <= ultimoDia; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    diasHabiles++;
            }

            var empleados = await _context.Empleados
                .Include(e => e.Departamento)
                .Where(e => e.Estado)
                .OrderBy(e => e.Nombres)
                .ToListAsync();

            var asistencias = await _context.Asistencias
                .Where(a => a.Fecha >= primerDia && a.Fecha <= ultimoDia)
                .Select(a => new { a.IdEmpleado, a.Fecha, a.TardanzaMinutos, a.HorasTrabajadas })
                .ToListAsync();

            var permisos = await _context.PermisosEmpleado
                .Where(p => p.Estado && p.FechaInicio <= ultimoDia && p.FechaFin >= primerDia)
                .Select(p => new { p.IdEmpleado, p.FechaInicio, p.FechaFin })
                .ToListAsync();

            var resumenes = empleados.Select(e =>
            {
                var asis = asistencias.Where(a => a.IdEmpleado == e.Id).ToList();
                var diasTrabajados = asis.Select(a => a.Fecha.Date).Distinct().Count();
                var totalHoras = asis.Sum(a => a.HorasTrabajadas);
                var tardanzas = asis.Count(a => a.TardanzaMinutos > 0);
                var minutosTardanza = asis.Sum(a => a.TardanzaMinutos);
                var horasExtras = asis.Where(a => a.HorasTrabajadas > 8).Sum(a => a.HorasTrabajadas - 8);

                var diasPermiso = new List<DateTime>();
                foreach (var p in permisos.Where(p => p.IdEmpleado == e.Id))
                {
                    var inicio = p.FechaInicio.Date < primerDia ? primerDia : p.FechaInicio.Date;
                    var fin = p.FechaFin.Date > ultimoDia ? ultimoDia : p.FechaFin.Date;
                    for (var d = inicio; d <= fin; d = d.AddDays(1))
                    {
                        if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday && !diasPermiso.Contains(d))
                            diasPermiso.Add(d);
                    }
                }

                var faltas = Math.Max(0, diasHabiles - diasTrabajados - diasPermiso.Count);

                return new AsistenciaResumenDto
                {
                    IdEmpleado = e.Id,
                    Empleado = e.Nombres + " " + e.Apellidos,
                    Departamento = e.Departamento?.Nombre ?? string.Empty,
                    Puesto = e.Puesto,
                    DiasHabiles = diasHabiles,
                    DiasTrabajados = diasTrabajados,
                    Faltas = faltas,
                    Tardanzas = tardanzas,
                    MinutosTardanza = minutosTardanza,
                    TotalHoras = Math.Round(totalHoras, 2),
                    HorasExtras = Math.Round(horasExtras, 2)
                };
            })
            .OrderByDescending(r => r.DiasTrabajados)
            .ThenBy(r => r.Empleado)
            .ToList();

            return Result<List<AsistenciaResumenDto>>.Ok(resumenes);
        }

        public async Task<Result<ExportResultDto>> ExportarResumenAsistenciasAsync(int anio, int mes)
        {
            var resultado = await ObtenerResumenAsistenciasAsync(anio, mes);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID", "Empleado", "Departamento", "Puesto", "Días hábiles", "Días trabajados", "Faltas", "Tardanzas", "Min. tardanza", "Horas trabajadas", "Horas extras" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.IdEmpleado, r.Empleado, r.Departamento, r.Puesto ?? "", r.DiasHabiles, r.DiasTrabajados, r.Faltas, r.Tardanzas, r.MinutosTardanza, r.TotalHoras, r.HorasExtras });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro($"Resumen de Asistencia - {mes:D2}/{anio}", tabla, "ResumenAsistencia"));
        }

        public async Task<Result<List<PermisoEmpleadoResponseDto>>> ObtenerPermisosAsync()
        {
            var permisos = await _context.PermisosEmpleado
                .Include(p => p.Empleado)
                .Include(p => p.TipoPermiso)
                .OrderByDescending(p => p.FechaInicio)
                .Select(p => new PermisoEmpleadoResponseDto
                {
                    Id = p.Id,
                    IdEmpleado = p.IdEmpleado,
                    Empleado = p.Empleado.Nombres + " " + p.Empleado.Apellidos,
                    IdTipoPermiso = p.IdTipoPermiso,
                    TipoPermiso = p.TipoPermiso.Nombre,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Motivo = p.Motivo,
                    DocumentoUrl = p.DocumentoUrl,
                    AutorizadoPor = p.AutorizadoPor,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<PermisoEmpleadoResponseDto>>.Ok(permisos);
        }

        public async Task<Result<PermisoEmpleadoResponseDto>> RegistrarPermisoAsync(PermisoEmpleadoCreateDto dto)
        {
            var empleadoExiste = await _context.Empleados.AnyAsync(e => e.Id == dto.IdEmpleado && e.Estado);
            var tipoPermisoExiste = await _context.TiposPermiso.AnyAsync(tp => tp.Id == dto.IdTipoPermiso && tp.Estado);

            if (!empleadoExiste)
                return Result<PermisoEmpleadoResponseDto>.Fail("El empleado no existe o está desactivado");
            if (!tipoPermisoExiste)
                return Result<PermisoEmpleadoResponseDto>.Fail("El tipo de permiso no es válido");

            if (dto.FechaFin < dto.FechaInicio)
                return Result<PermisoEmpleadoResponseDto>.Fail("La fecha de fin debe ser posterior a la fecha de inicio");

            var permiso = new PermisoEmpleado
            {
                IdEmpleado = dto.IdEmpleado,
                IdTipoPermiso = dto.IdTipoPermiso,
                FechaInicio = dto.FechaInicio.Date,
                FechaFin = dto.FechaFin.Date,
                Motivo = dto.Motivo.Trim(),
                DocumentoUrl = dto.DocumentoUrl?.Trim(),
                AutorizadoPor = dto.AutorizadoPor?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.PermisosEmpleado.Add(permiso);
            await _context.SaveChangesAsync();

            var perm = await _context.PermisosEmpleado
                .Include(p => p.Empleado)
                .Include(p => p.TipoPermiso)
                .Where(p => p.Id == permiso.Id)
                .Select(p => new PermisoEmpleadoResponseDto
                {
                    Id = p.Id,
                    IdEmpleado = p.IdEmpleado,
                    Empleado = p.Empleado.Nombres + " " + p.Empleado.Apellidos,
                    IdTipoPermiso = p.IdTipoPermiso,
                    TipoPermiso = p.TipoPermiso.Nombre,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Motivo = p.Motivo,
                    DocumentoUrl = p.DocumentoUrl,
                    AutorizadoPor = p.AutorizadoPor,
                    Estado = p.Estado
                })
                .FirstOrDefaultAsync();

            return perm != null
                ? Result<PermisoEmpleadoResponseDto>.Ok(perm)
                : Result<PermisoEmpleadoResponseDto>.Fail("No se pudo registrar el permiso");
        }
    }
}
