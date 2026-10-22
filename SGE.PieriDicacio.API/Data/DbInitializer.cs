using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Rol>
                {
                    new Rol { Nombre = "Administrador", Descripcion = "Control total del sistema", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Rol { Nombre = "Administración", Descripcion = "Gestión administrativa", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Rol { Nombre = "Producción", Descripcion = "Gestión de pedidos y producción", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Rol { Nombre = "Bodega", Descripcion = "Control de inventarios", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Rol { Nombre = "RRHH", Descripcion = "Control de asistencia y personal", Estado = true, FechaCreacion = DateTime.UtcNow }
                };

                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }

            var rolAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Administrador");
            var usuarioAdmin = await context.Usuarios.FirstOrDefaultAsync(u => u.Username == "admin");
            
            if (usuarioAdmin == null)
            {
                usuarioAdmin = new Usuario
                {
                    IdRol = rolAdmin.Id,
                    Nombres = "Admin",
                    Apellidos = "General",
                    Username = "admin",
                    Correo = "admin@pieri.com",
                    Telefono = "0000-0000",
                    PasswordHash = PasswordHelper.HashPassword("Admin123*"),
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow
                };
                context.Usuarios.Add(usuarioAdmin);
            }
            else
            {
                // Actualizar el hash de la contraseña si el usuario ya existe
                usuarioAdmin.PasswordHash = PasswordHelper.HashPassword("Admin123*");
                usuarioAdmin.Estado = true;
            }
            await context.SaveChangesAsync();

            await SeedDepartamentosAsync(context);
            await SeedEstadosPedidoAsync(context);
            await SeedEstadosFinancierosAsync(context);
            await SeedMetodosPagoAsync(context);
            await SeedTiposPrendaAsync(context);
            await SeedTallasAsync(context);
            await SeedColoresAsync(context);
            await SeedUnidadesMedidaAsync(context);
            await SeedEstadosOrdenProduccionAsync(context);
            await SeedTiposMovimientoInventarioAsync(context);
            await SeedTiposPermisoAsync(context);
            await SeedPermisosYAsignacionAsync(context);
        }

        private static async Task SeedDepartamentosAsync(ApplicationDbContext context)
        {
            if (!await context.Departamentos.AnyAsync())
            {
                var departamentos = new List<Departamento>
                {
                    new Departamento { Nombre = "Administración", Descripcion = "Gestión administrativa y reportes", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Departamento { Nombre = "Producción", Descripcion = "Área de producción y operaciones", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Departamento { Nombre = "Bodega", Descripcion = "Control de stock e inventario", Estado = true, FechaCreacion = DateTime.UtcNow },
                    new Departamento { Nombre = "Recursos Humanos", Descripcion = "Control de personal y asistencia", Estado = true, FechaCreacion = DateTime.UtcNow }
                };

                context.Departamentos.AddRange(departamentos);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedEstadosPedidoAsync(ApplicationDbContext context)
        {
            if (!await context.EstadosPedido.AnyAsync())
            {
                var estados = new List<EstadoPedido>
                {
                    new EstadoPedido { Nombre = "Registrado", Descripcion = "Pedido ingresado al sistema", Orden = 1, Estado = true },
                    new EstadoPedido { Nombre = "En revisión", Descripcion = "Pedido en validación o revisión", Orden = 2, Estado = true },
                    new EstadoPedido { Nombre = "En producción", Descripcion = "Pedido en proceso de elaboración", Orden = 3, Estado = true },
                    new EstadoPedido { Nombre = "Listo", Descripcion = "Pedido terminado y listo para entrega", Orden = 4, Estado = true },
                    new EstadoPedido { Nombre = "Entregado", Descripcion = "Pedido entregado al cliente", Orden = 5, Estado = true },
                    new EstadoPedido { Nombre = "Cancelado", Descripcion = "Pedido cancelado", Orden = 6, Estado = true },
                    new EstadoPedido { Nombre = "Suspendido", Descripcion = "Pedido detenido temporalmente", Orden = 7, Estado = true }
                };

                context.EstadosPedido.AddRange(estados);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedEstadosFinancierosAsync(ApplicationDbContext context)
        {
            if (!await context.EstadosFinancierosPedido.AnyAsync())
            {
                var estados = new List<PedidoEstadoFinanciero>
                {
                    new PedidoEstadoFinanciero { Nombre = "Pendiente", Descripcion = "No tiene pagos", Estado = true },
                    new PedidoEstadoFinanciero { Nombre = "Parcial", Descripcion = "Tiene pagos parciales", Estado = true },
                    new PedidoEstadoFinanciero { Nombre = "Pagado", Descripcion = "Pago completado", Estado = true }
                };

                context.EstadosFinancierosPedido.AddRange(estados);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedMetodosPagoAsync(ApplicationDbContext context)
        {
            if (!await context.MetodosPago.AnyAsync())
            {
                var metodos = new List<MetodoPago>
                {
                    new MetodoPago { Nombre = "Efectivo", Descripcion = "Pago en efectivo", Estado = true },
                    new MetodoPago { Nombre = "Tarjeta", Descripcion = "Pago con tarjeta de débito o crédito", Estado = true },
                    new MetodoPago { Nombre = "Boleta o comprobante", Descripcion = "Depósito o transferencia bancaria", Estado = true }
                };

                context.MetodosPago.AddRange(metodos);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTiposPrendaAsync(ApplicationDbContext context)
        {
            if (!await context.TiposPrenda.AnyAsync())
            {
                var tiposPrenda = new List<TipoPrenda>
                {
                    new TipoPrenda { Nombre = "Camisa", Descripcion = "Camisa de vestir", Estado = true },
                    new TipoPrenda { Nombre = "Pantalón", Descripcion = "Pantalón de vestir", Estado = true },
                    new TipoPrenda { Nombre = "Gabacha", Descripcion = "Gabacha o chaqueta", Estado = true },
                    new TipoPrenda { Nombre = "Uniforme completo", Descripcion = "Conjunto completo de uniforme", Estado = true },
                    new TipoPrenda { Nombre = "Camisa tipo polo", Descripcion = "Camisa tipo polo", Estado = true },
                    new TipoPrenda { Nombre = "Chaleco", Descripcion = "Chaleco", Estado = true },
                    new TipoPrenda { Nombre = "Falda", Descripcion = "Falda", Estado = true },
                    new TipoPrenda { Nombre = "Vestido", Descripcion = "Vestido", Estado = true }
                };

                context.TiposPrenda.AddRange(tiposPrenda);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTallasAsync(ApplicationDbContext context)
        {
            if (!await context.Tallas.AnyAsync())
            {
                var tallas = new List<Talla>
                {
                    new Talla { Nombre = "XS", Descripcion = "Extra pequeña", Estado = true },
                    new Talla { Nombre = "S", Descripcion = "Pequeña", Estado = true },
                    new Talla { Nombre = "M", Descripcion = "Mediana", Estado = true },
                    new Talla { Nombre = "L", Descripcion = "Grande", Estado = true },
                    new Talla { Nombre = "XL", Descripcion = "Extra grande", Estado = true },
                    new Talla { Nombre = "XXL", Descripcion = "Doble extra grande", Estado = true },
                    new Talla { Nombre = "XXXL", Descripcion = "Triple extra grande", Estado = true },
                    new Talla { Nombre = "28", Descripcion = "Talla 28", Estado = true },
                    new Talla { Nombre = "30", Descripcion = "Talla 30", Estado = true },
                    new Talla { Nombre = "32", Descripcion = "Talla 32", Estado = true },
                    new Talla { Nombre = "34", Descripcion = "Talla 34", Estado = true },
                    new Talla { Nombre = "36", Descripcion = "Talla 36", Estado = true },
                    new Talla { Nombre = "38", Descripcion = "Talla 38", Estado = true },
                    new Talla { Nombre = "40", Descripcion = "Talla 40", Estado = true },
                    new Talla { Nombre = "42", Descripcion = "Talla 42", Estado = true },
                    new Talla { Nombre = "44", Descripcion = "Talla 44", Estado = true },
                    new Talla { Nombre = "Talla única", Descripcion = "Talla única", Estado = true }
                };

                context.Tallas.AddRange(tallas);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedColoresAsync(ApplicationDbContext context)
        {
            if (!await context.Colores.AnyAsync())
            {
                var colores = new List<Color>
                {
                    new Color { Nombre = "Negro", Descripcion = "Color negro", Estado = true },
                    new Color { Nombre = "Blanco", Descripcion = "Color blanco", Estado = true },
                    new Color { Nombre = "Azul", Descripcion = "Color azul", Estado = true },
                    new Color { Nombre = "Rojo", Descripcion = "Color rojo", Estado = true },
                    new Color { Nombre = "Gris", Descripcion = "Color gris", Estado = true },
                    new Color { Nombre = "Verde", Descripcion = "Color verde", Estado = true },
                    new Color { Nombre = "Amarillo", Descripcion = "Color amarillo", Estado = true },
                    new Color { Nombre = "Naranja", Descripcion = "Color naranja", Estado = true },
                    new Color { Nombre = "Morado", Descripcion = "Color morado", Estado = true },
                    new Color { Nombre = "Rosa", Descripcion = "Color rosa", Estado = true },
                    new Color { Nombre = "Café", Descripcion = "Color café/marrón", Estado = true },
                    new Color { Nombre = "Beige", Descripcion = "Color beige", Estado = true },
                    new Color { Nombre = "Vino", Descripcion = "Color vino/borgoña", Estado = true },
                    new Color { Nombre = "Turquesa", Descripcion = "Color turquesa", Estado = true },
                    new Color { Nombre = "Mostaza", Descripcion = "Color mostaza", Estado = true }
                };

                context.Colores.AddRange(colores);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUnidadesMedidaAsync(ApplicationDbContext context)
        {
            if (!await context.UnidadesMedida.AnyAsync())
            {
                var unidades = new List<UnidadMedida>
                {
                    new UnidadMedida { Nombre = "Unidad", Abreviatura = "Und", Descripcion = "Unidad individual", Estado = true },
                    new UnidadMedida { Nombre = "Metro", Abreviatura = "m", Descripcion = "Metro lineal", Estado = true },
                    new UnidadMedida { Nombre = "Kilogramo", Abreviatura = "kg", Descripcion = "Kilogramo", Estado = true },
                    new UnidadMedida { Nombre = "Gramo", Abreviatura = "g", Descripcion = "Gramo", Estado = true },
                    new UnidadMedida { Nombre = "Litro", Abreviatura = "L", Descripcion = "Litro", Estado = true },
                    new UnidadMedida { Nombre = "Millilitro", Abreviatura = "mL", Descripcion = "Millilitro", Estado = true },
                    new UnidadMedida { Nombre = "Rollo", Abreviatura = "Rol", Descripcion = "Rollo de tela", Estado = true },
                    new UnidadMedida { Nombre = "Caja", Abreviatura = "Cja", Descripcion = "Caja", Estado = true },
                    new UnidadMedida { Nombre = "Paquete", Abreviatura = "Paq", Descripcion = "Paquete", Estado = true },
                    new UnidadMedida { Nombre = "Docena", Abreviatura = "Doc", Descripcion = "Docena (12 unidades)", Estado = true },
                    new UnidadMedida { Nombre = "Centímetro", Abreviatura = "cm", Descripcion = "Centímetro", Estado = true },
                    new UnidadMedida { Nombre = "Yarda", Abreviatura = "yd", Descripcion = "Yarda", Estado = true },
                    new UnidadMedida { Nombre = "Par", Abreviatura = "Par", Descripcion = "Par (2 unidades)", Estado = true },
                    new UnidadMedida { Nombre = "Juego", Abreviatura = "Jgo", Descripcion = "Juego completo", Estado = true }
                };

                context.UnidadesMedida.AddRange(unidades);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedEstadosOrdenProduccionAsync(ApplicationDbContext context)
        {
            if (!await context.EstadosOrdenProduccion.AnyAsync())
            {
                var estados = new List<EstadoOrdenProduccion>
                {
                    new EstadoOrdenProduccion { Nombre = "Pendiente", Descripcion = "Orden de producción pendiente de iniciar", Orden = 1, Estado = true },
                    new EstadoOrdenProduccion { Nombre = "En Proceso", Descripcion = "Orden de producción en ejecución", Orden = 2, Estado = true },
                    new EstadoOrdenProduccion { Nombre = "Completada", Descripcion = "Orden de producción completada", Orden = 3, Estado = true },
                    new EstadoOrdenProduccion { Nombre = "Cancelada", Descripcion = "Orden de producción cancelada", Orden = 4, Estado = true }
                };

                context.EstadosOrdenProduccion.AddRange(estados);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTiposMovimientoInventarioAsync(ApplicationDbContext context)
        {
            if (!await context.TiposMovimientoInventario.AnyAsync())
            {
                var tipos = new List<TipoMovimientoInventario>
                {
                    new TipoMovimientoInventario { Nombre = "Entrada", Descripcion = "Ingreso de existencia", AfectaStock = true, TipoEfecto = "SUMA", Estado = true },
                    new TipoMovimientoInventario { Nombre = "Salida", Descripcion = "Salida de existencia", AfectaStock = true, TipoEfecto = "RESTA", Estado = true },
                    new TipoMovimientoInventario { Nombre = "Ajuste", Descripcion = "Ajuste manual por inventario físico", AfectaStock = true, TipoEfecto = "NEUTRO", Estado = true },
                    new TipoMovimientoInventario { Nombre = "Merma", Descripcion = "Pérdida o daño de producto", AfectaStock = true, TipoEfecto = "RESTA", Estado = true }
                };

                context.TiposMovimientoInventario.AddRange(tipos);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTiposPermisoAsync(ApplicationDbContext context)
        {
            if (!await context.TiposPermiso.AnyAsync())
            {
                var tipos = new List<TipoPermiso>
                {
                    new TipoPermiso { Nombre = "Permiso", Descripcion = "Permiso autorizado", Estado = true },
                    new TipoPermiso { Nombre = "Ausencia", Descripcion = "Ausencia sin asistencia", Estado = true },
                    new TipoPermiso { Nombre = "Incapacidad", Descripcion = "Ausencia por incapacidad", Estado = true },
                    new TipoPermiso { Nombre = "Vacaciones", Descripcion = "Periodo vacacional", Estado = true }
                };

                context.TiposPermiso.AddRange(tipos);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedPermisosYAsignacionAsync(ApplicationDbContext context)
        {
            var permisosBase = new List<Permiso>
            {
                new Permiso { Nombre = "Gestionar Usuarios", Modulo = "Usuarios", Descripcion = "Crear, editar y desactivar usuarios", Estado = true },
                new Permiso { Nombre = "Gestionar Roles y Permisos", Modulo = "Roles", Descripcion = "Administrar roles y sus permisos", Estado = true },
                new Permiso { Nombre = "Ver Bitácora", Modulo = "Bitácora", Descripcion = "Consultar el registro de auditoría", Estado = true },

                new Permiso { Nombre = "Ver Clientes", Modulo = "Clientes", Descripcion = "Consultar clientes", Estado = true },
                new Permiso { Nombre = "Gestionar Clientes", Modulo = "Clientes", Descripcion = "Crear y editar clientes", Estado = true },

                new Permiso { Nombre = "Ver Pedidos", Modulo = "Pedidos", Descripcion = "Consultar pedidos", Estado = true },
                new Permiso { Nombre = "Crear Pedidos", Modulo = "Pedidos", Descripcion = "Registrar nuevos pedidos", Estado = true },
                new Permiso { Nombre = "Cambiar Estado Pedido", Modulo = "Pedidos", Descripcion = "Avanzar el estado de un pedido", Estado = true },

                new Permiso { Nombre = "Ver Pagos", Modulo = "Pagos", Descripcion = "Consultar pagos y resúmenes", Estado = true },
                new Permiso { Nombre = "Registrar Pagos", Modulo = "Pagos", Descripcion = "Registrar abonos y pagos", Estado = true },

                new Permiso { Nombre = "Ver Inventario", Modulo = "Inventario", Descripcion = "Consultar productos, insumos y movimientos", Estado = true },
                new Permiso { Nombre = "Gestionar Inventario", Modulo = "Inventario", Descripcion = "Registrar productos, insumos y movimientos", Estado = true },

                new Permiso { Nombre = "Ver Producción", Modulo = "Producción", Descripcion = "Consultar órdenes de producción", Estado = true },
                new Permiso { Nombre = "Gestionar Producción", Modulo = "Producción", Descripcion = "Crear y ejecutar órdenes de producción", Estado = true },
                new Permiso { Nombre = "Gestionar Lista de Materiales", Modulo = "BOM", Descripcion = "Administrar recetas (BOM) y verificar disponibilidad", Estado = true },

                new Permiso { Nombre = "Ver Empleados", Modulo = "Empleados", Descripcion = "Consultar empleados", Estado = true },
                new Permiso { Nombre = "Gestionar Empleados", Modulo = "Empleados", Descripcion = "Crear y editar empleados y permisos", Estado = true },
                new Permiso { Nombre = "Registrar Asistencia", Modulo = "Empleados", Descripcion = "Registrar asistencias", Estado = true },

                new Permiso { Nombre = "Ver Reportes", Modulo = "Reportes", Descripcion = "Consultar reportes y dashboard", Estado = true },
                new Permiso { Nombre = "Gestionar Catálogos", Modulo = "Catálogos", Descripcion = "Administrar tallas, colores, prendas y unidades", Estado = true }
            };

            var nombresPermisosExistentes = (await context.Permisos
                    .Select(p => p.Nombre)
                    .ToListAsync())
                .ToHashSet(StringComparer.Ordinal);

            var permisosFaltantes = permisosBase
                .Where(p => !nombresPermisosExistentes.Contains(p.Nombre))
                .ToList();

            if (permisosFaltantes.Count > 0)
            {
                context.Permisos.AddRange(permisosFaltantes);
                await context.SaveChangesAsync();
            }

            var permisos = await context.Permisos.ToListAsync();
            var roles = await context.Roles.ToListAsync();
            var noHabiaAsignaciones = !await context.RolesPermisos.AnyAsync();

            var asignacion = noHabiaAsignaciones
                ? new Dictionary<string, string[]>
                {
                    ["Administrador"] = permisos.Select(p => p.Nombre).ToArray(),
                    ["Administración"] = new[]
                    {
                        "Ver Clientes", "Gestionar Clientes",
                        "Ver Pedidos", "Crear Pedidos", "Cambiar Estado Pedido",
                        "Ver Pagos", "Registrar Pagos",
                        "Ver Reportes", "Gestionar Catálogos",
                        "Ver Empleados"
                    },
                    ["Producción"] = new[]
                    {
                        "Ver Pedidos", "Cambiar Estado Pedido",
                        "Ver Producción", "Gestionar Producción",
                        "Gestionar Lista de Materiales", "Ver Inventario"
                    },
                    ["Bodega"] = new[]
                    {
                        "Ver Inventario", "Gestionar Inventario", "Ver Reportes"
                    },
                    ["RRHH"] = new[]
                    {
                        "Ver Empleados", "Gestionar Empleados", "Registrar Asistencia", "Ver Reportes"
                    }
                }
                : new Dictionary<string, string[]>
                {
                    // El rol Administrador representa control total: recibe también
                    // cualquier permiso incorporado después de crear la base de datos.
                    ["Administrador"] = permisos.Select(p => p.Nombre).ToArray()
                };

            var asignacionesExistentes = (await context.RolesPermisos
                    .Select(rp => new { rp.IdRol, rp.IdPermiso })
                    .ToListAsync())
                .Select(rp => (rp.IdRol, rp.IdPermiso))
                .ToHashSet();

            foreach (var entrada in asignacion)
            {
                var rol = roles.First(r => r.Nombre == entrada.Key);
                foreach (var nombrePermiso in entrada.Value)
                {
                    var permiso = permisos.First(p => p.Nombre == nombrePermiso);
                    if (!asignacionesExistentes.Add((rol.Id, permiso.Id)))
                    {
                        continue;
                    }

                    context.RolesPermisos.Add(new RolPermiso
                    {
                        IdRol = rol.Id,
                        IdPermiso = permiso.Id,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
