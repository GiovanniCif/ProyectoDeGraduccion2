using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Cliente;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;
        private static readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex _phoneRegex = new Regex(@"^[\d\s\-\+\(\)]{6,20}$", RegexOptions.Compiled);

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ClienteResponseDto>>> ObtenerTodosAsync()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.NombreRazonSocial)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    TipoCliente = c.TipoCliente,
                    NombreRazonSocial = c.NombreRazonSocial,
                    NombreContacto = c.NombreContacto,
                    Nit = c.Nit,
                    TelefonoPrincipal = c.TelefonoPrincipal,
                    TelefonoSecundario = c.TelefonoSecundario,
                    Correo = c.Correo,
                    Direccion = c.Direccion,
                    Municipio = c.Municipio,
                    Departamento = c.Departamento,
                    Observaciones = c.Observaciones,
                    Estado = c.Estado
                })
                .ToListAsync();

            return Result<List<ClienteResponseDto>>.Ok(clientes);
        }

        public async Task<Result<ClienteResponseDto>> ObtenerPorIdAsync(int id)
        {
            var cliente = await _context.Clientes
                .Where(c => c.Id == id)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    TipoCliente = c.TipoCliente,
                    NombreRazonSocial = c.NombreRazonSocial,
                    NombreContacto = c.NombreContacto,
                    Nit = c.Nit,
                    TelefonoPrincipal = c.TelefonoPrincipal,
                    TelefonoSecundario = c.TelefonoSecundario,
                    Correo = c.Correo,
                    Direccion = c.Direccion,
                    Municipio = c.Municipio,
                    Departamento = c.Departamento,
                    Observaciones = c.Observaciones,
                    Estado = c.Estado
                })
                .FirstOrDefaultAsync();

            return cliente != null
                ? Result<ClienteResponseDto>.Ok(cliente)
                : Result<ClienteResponseDto>.Fail("Cliente no encontrado");
        }

        public async Task<Result<ClienteResponseDto>> CrearAsync(ClienteCreateDto dto)
        {
            var validation = ValidarCamposCliente(dto.TelefonoPrincipal, dto.TelefonoSecundario, dto.Correo);
            if (!validation.Success)
                return Result<ClienteResponseDto>.Fail(validation.Message);

            var cliente = new Cliente
            {
                TipoCliente = dto.TipoCliente.Trim().ToUpper(),
                NombreRazonSocial = dto.NombreRazonSocial.Trim(),
                NombreContacto = NormalizarCampo(dto.NombreContacto),
                Nit = NormalizarCampo(dto.Nit),
                TelefonoPrincipal = NormalizarCampo(dto.TelefonoPrincipal),
                TelefonoSecundario = NormalizarCampo(dto.TelefonoSecundario),
                Correo = NormalizarCampo(dto.Correo),
                Direccion = NormalizarCampo(dto.Direccion),
                Municipio = NormalizarCampo(dto.Municipio),
                Departamento = NormalizarCampo(dto.Departamento),
                Observaciones = NormalizarCampo(dto.Observaciones),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Result<ClienteResponseDto>.Ok(new ClienteResponseDto
            {
                Id = cliente.Id,
                TipoCliente = cliente.TipoCliente,
                NombreRazonSocial = cliente.NombreRazonSocial,
                NombreContacto = cliente.NombreContacto,
                Nit = cliente.Nit,
                TelefonoPrincipal = cliente.TelefonoPrincipal,
                TelefonoSecundario = cliente.TelefonoSecundario,
                Correo = cliente.Correo,
                Direccion = cliente.Direccion,
                Municipio = cliente.Municipio,
                Departamento = cliente.Departamento,
                Observaciones = cliente.Observaciones,
                Estado = cliente.Estado
            });
        }

        private static string? NormalizarCampo(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;
            return valor.Trim();
        }

        private static (bool Success, string Message) ValidarCamposCliente(string? telefonoPrincipal, string? telefonoSecundario, string? correo)
        {
            if (!string.IsNullOrWhiteSpace(correo))
            {
                var correoTrim = correo.Trim();
                if (!_emailRegex.IsMatch(correoTrim))
                    return (false, $"El correo '{correoTrim}' no tiene un formato válido");
            }

            if (!string.IsNullOrWhiteSpace(telefonoPrincipal))
            {
                var telTrim = telefonoPrincipal.Trim();
                if (!_phoneRegex.IsMatch(telTrim))
                    return (false, $"El teléfono principal '{telTrim}' no tiene un formato válido");
            }

            if (!string.IsNullOrWhiteSpace(telefonoSecundario))
            {
                var telTrim = telefonoSecundario.Trim();
                if (!_phoneRegex.IsMatch(telTrim))
                    return (false, $"El teléfono secundario '{telTrim}' no tiene un formato válido");
            }

            return (true, string.Empty);
        }

        public async Task<Result<ClienteResponseDto>> ActualizarAsync(int id, ClienteUpdateDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return Result<ClienteResponseDto>.Fail("Cliente no encontrado");

            var validation = ValidarCamposCliente(dto.TelefonoPrincipal, dto.TelefonoSecundario, dto.Correo);
            if (!validation.Success)
                return Result<ClienteResponseDto>.Fail(validation.Message);

            cliente.TipoCliente = dto.TipoCliente.Trim().ToUpper();
            cliente.NombreRazonSocial = dto.NombreRazonSocial.Trim();
            cliente.NombreContacto = NormalizarCampo(dto.NombreContacto);
            cliente.Nit = NormalizarCampo(dto.Nit);
            cliente.TelefonoPrincipal = NormalizarCampo(dto.TelefonoPrincipal);
            cliente.TelefonoSecundario = NormalizarCampo(dto.TelefonoSecundario);
            cliente.Correo = NormalizarCampo(dto.Correo);
            cliente.Direccion = NormalizarCampo(dto.Direccion);
            cliente.Municipio = NormalizarCampo(dto.Municipio);
            cliente.Departamento = NormalizarCampo(dto.Departamento);
            cliente.Observaciones = NormalizarCampo(dto.Observaciones);
            cliente.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result<ClienteResponseDto>.Ok(new ClienteResponseDto
            {
                Id = cliente.Id,
                TipoCliente = cliente.TipoCliente,
                NombreRazonSocial = cliente.NombreRazonSocial,
                NombreContacto = cliente.NombreContacto,
                Nit = cliente.Nit,
                TelefonoPrincipal = cliente.TelefonoPrincipal,
                TelefonoSecundario = cliente.TelefonoSecundario,
                Correo = cliente.Correo,
                Direccion = cliente.Direccion,
                Municipio = cliente.Municipio,
                Departamento = cliente.Departamento,
                Observaciones = cliente.Observaciones,
                Estado = cliente.Estado
            });
        }

        public async Task<Result<bool>> CambiarEstadoAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return Result<bool>.Fail("Cliente no encontrado");

            cliente.Estado = !cliente.Estado;
            cliente.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result<bool>.Ok(true);
        }
    }
}
