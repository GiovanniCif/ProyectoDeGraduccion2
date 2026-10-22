using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly ApplicationDbContext _context;

        public BitacoraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<BitacoraResponseDto>> RegistrarAsync(BitacoraCreateDto dto, int idUsuario)
        {
            var bitacora = new Bitacora
            {
                IdUsuario = idUsuario,
                Modulo = dto.Modulo.Trim(),
                Accion = dto.Accion.Trim(),
                TablaAfectada = dto.TablaAfectada.Trim(),
                IdRegistroAfectado = dto.IdRegistroAfectado,
                Descripcion = dto.Descripcion?.Trim(),
                DatosAnteriores = dto.DatosAnteriores,
                DatosNuevos = dto.DatosNuevos,
                IpEquipo = dto.IpEquipo?.Trim(),
                FechaEvento = DateTime.UtcNow
            };

            _context.Bitacoras.Add(bitacora);
            await _context.SaveChangesAsync();

            var b = await _context.Bitacoras
                .Include(b => b.Usuario)
                .Where(b => b.Id == bitacora.Id)
                .Select(b => new BitacoraResponseDto
                {
                    Id = b.Id,
                    IdUsuario = b.IdUsuario,
                    Usuario = b.Usuario.Username,
                    Modulo = b.Modulo,
                    Accion = b.Accion,
                    TablaAfectada = b.TablaAfectada,
                    IdRegistroAfectado = b.IdRegistroAfectado,
                    Descripcion = b.Descripcion,
                    DatosAnteriores = b.DatosAnteriores,
                    DatosNuevos = b.DatosNuevos,
                    IpEquipo = b.IpEquipo,
                    FechaEvento = b.FechaEvento
                })
                .FirstOrDefaultAsync();

            return b != null
                ? Result<BitacoraResponseDto>.Ok(b)
                : Result<BitacoraResponseDto>.Fail("No se pudo registrar la bitácora");
        }

        public async Task<Result<List<BitacoraResponseDto>>> ObtenerTodosAsync()
        {
            var bitacoras = await _context.Bitacoras
                .Include(b => b.Usuario)
                .OrderByDescending(b => b.FechaEvento)
                .Select(b => new BitacoraResponseDto
                {
                    Id = b.Id,
                    IdUsuario = b.IdUsuario,
                    Usuario = b.Usuario.Username,
                    Modulo = b.Modulo,
                    Accion = b.Accion,
                    TablaAfectada = b.TablaAfectada,
                    IdRegistroAfectado = b.IdRegistroAfectado,
                    Descripcion = b.Descripcion,
                    DatosAnteriores = b.DatosAnteriores,
                    DatosNuevos = b.DatosNuevos,
                    IpEquipo = b.IpEquipo,
                    FechaEvento = b.FechaEvento
                })
                .ToListAsync();

            return Result<List<BitacoraResponseDto>>.Ok(bitacoras);
        }
    }
}
