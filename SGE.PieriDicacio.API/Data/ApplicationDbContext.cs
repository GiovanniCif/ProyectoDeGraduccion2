using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Entities;

namespace SGE.PieriDicacio.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolesPermisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidosDetalle { get; set; }
        public DbSet<PedidoHistorialEstado> PedidosHistorialEstado { get; set; }
        public DbSet<EstadoPedido> EstadosPedido { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<TipoPrenda> TiposPrenda { get; set; }
        public DbSet<Talla> Tallas { get; set; }
        public DbSet<Color> Colores { get; set; }

        public DbSet<Pago> Pagos { get; set; }
        public DbSet<PedidoResumenPago> PedidosResumenPago { get; set; }
        public DbSet<PedidoEstadoFinanciero> EstadosFinancierosPedido { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<TipoMovimientoInventario> TiposMovimientoInventario { get; set; }
        public DbSet<ProductoTerminado> ProductosTerminados { get; set; }
        public DbSet<MovimientoProductoTerminado> MovimientosProductoTerminado { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<MovimientoInsumo> MovimientosInsumo { get; set; }

        public DbSet<EstadoOrdenProduccion> EstadosOrdenProduccion { get; set; }
        public DbSet<BOMDetalle> BOMDetalles { get; set; }
        public DbSet<OrdenProduccion> OrdenesProduccion { get; set; }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<TipoPermiso> TiposPermiso { get; set; }
        public DbSet<PermisoEmpleado> PermisosEmpleado { get; set; }

        public DbSet<Bitacora> Bitacoras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("rol");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.ToTable("permiso");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Modulo).HasColumnName("modulo");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<RolPermiso>(entity =>
            {
                entity.ToTable("rol_permiso");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdRol).HasColumnName("id_rol");
                entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

                entity.HasIndex(e => new { e.IdRol, e.IdPermiso })
                    .IsUnique();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.RolesPermisos)
                    .HasForeignKey(e => e.IdRol);

                entity.HasOne(e => e.Permiso)
                    .WithMany(p => p.RolesPermisos)
                    .HasForeignKey(e => e.IdPermiso);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdRol).HasColumnName("id_rol");
                entity.Property(e => e.Nombres).HasColumnName("nombres");
                entity.Property(e => e.Apellidos).HasColumnName("apellidos");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Correo).HasColumnName("correo");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.UltimoAcceso).HasColumnName("ultimo_acceso");
                entity.Property(e => e.IntentosFallidos).HasColumnName("intentos_fallidos");
                entity.Property(e => e.BloqueadoHasta).HasColumnName("bloqueado_hasta");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Correo).IsUnique();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.IdRol);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TipoCliente).HasColumnName("tipo_cliente");
                entity.Property(e => e.NombreRazonSocial).HasColumnName("nombre_razon_social");
                entity.Property(e => e.NombreContacto).HasColumnName("nombre_contacto");
                entity.Property(e => e.Nit).HasColumnName("nit");
                entity.Property(e => e.TelefonoPrincipal).HasColumnName("telefono_principal");
                entity.Property(e => e.TelefonoSecundario).HasColumnName("telefono_secundario");
                entity.Property(e => e.Correo).HasColumnName("correo");
                entity.Property(e => e.Direccion).HasColumnName("direccion");
                entity.Property(e => e.Municipio).HasColumnName("municipio");
                entity.Property(e => e.Departamento).HasColumnName("departamento");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("pedido");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.NumeroPedido).HasColumnName("numero_pedido");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
                entity.Property(e => e.IdEstadoPedido).HasColumnName("id_estado_pedido");
                entity.Property(e => e.IdDepartamentoResponsable).HasColumnName("id_departamento_responsable");
                entity.Property(e => e.IdUsuarioResponsable).HasColumnName("id_usuario_responsable");
                entity.Property(e => e.FechaPedido).HasColumnName("fecha_pedido");
                entity.Property(e => e.FechaEntrega).HasColumnName("fecha_entrega");
                entity.Property(e => e.Prioridad).HasColumnName("prioridad");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal");
                entity.Property(e => e.Descuento).HasColumnName("descuento");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasOne(e => e.Cliente)
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente);

                entity.HasOne(e => e.EstadoPedido)
                    .WithMany(e => e.Pedidos)
                    .HasForeignKey(e => e.IdEstadoPedido);

                entity.HasOne(e => e.DepartamentoResponsable)
                    .WithMany()
                    .HasForeignKey(e => e.IdDepartamentoResponsable);

                entity.HasOne(e => e.UsuarioResponsable)
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuarioResponsable);

                entity.HasIndex(e => e.NumeroPedido).IsUnique();
            });

            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.ToTable("pedido_detalle");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.IdProductoTerminado).HasColumnName("id_producto_terminado");
                entity.Property(e => e.IdTipoPrenda).HasColumnName("id_tipo_prenda");
                entity.Property(e => e.DescripcionPrenda).HasColumnName("descripcion_prenda");
                entity.Property(e => e.IdTalla).HasColumnName("id_talla");
                entity.Property(e => e.IdColor).HasColumnName("id_color");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Detalles)
                    .HasForeignKey(e => e.IdPedido);

                entity.HasOne(e => e.ProductoTerminado)
                    .WithMany()
                    .HasForeignKey(e => e.IdProductoTerminado);

                entity.HasOne(e => e.TipoPrenda)
                    .WithMany()
                    .HasForeignKey(e => e.IdTipoPrenda);

                entity.HasOne(e => e.Talla)
                    .WithMany()
                    .HasForeignKey(e => e.IdTalla);

                entity.HasOne(e => e.Color)
                    .WithMany()
                    .HasForeignKey(e => e.IdColor);
            });

            modelBuilder.Entity<PedidoHistorialEstado>(entity =>
            {
                entity.ToTable("pedido_historial_estado");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.IdEstadoAnterior).HasColumnName("id_estado_anterior");
                entity.Property(e => e.IdEstadoNuevo).HasColumnName("id_estado_nuevo");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
                entity.Property(e => e.Comentario).HasColumnName("comentario");
                entity.Property(e => e.FechaCambio).HasColumnName("fecha_cambio");

                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.HistorialEstados)
                    .HasForeignKey(e => e.IdPedido);

                entity.HasOne(e => e.EstadoAnterior)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstadoAnterior)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EstadoNuevo)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstadoNuevo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuario);

                entity.HasOne(e => e.Departamento)
                    .WithMany()
                    .HasForeignKey(e => e.IdDepartamento);
            });

            modelBuilder.Entity<EstadoPedido>(entity =>
            {
                entity.ToTable("estado_pedido");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Orden).HasColumnName("orden");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("departamento");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            });

            modelBuilder.Entity<TipoPrenda>(entity =>
            {
                entity.ToTable("tipo_prenda");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<Talla>(entity =>
            {
                entity.ToTable("talla");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("color");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("pago");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
                entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
                entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
                entity.Property(e => e.Monto).HasColumnName("monto");
                entity.Property(e => e.NumeroReferencia).HasColumnName("numero_referencia");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.VoucherUrl).HasColumnName("voucher_url");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

                entity.HasOne(e => e.Pedido)
                    .WithMany()
                    .HasForeignKey(e => e.IdPedido);

                entity.HasOne(e => e.Cliente)
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente);

                entity.HasOne(e => e.MetodoPago)
                    .WithMany()
                    .HasForeignKey(e => e.IdMetodoPago);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuario);
            });

            modelBuilder.Entity<PedidoResumenPago>(entity =>
            {
                entity.ToTable("pedido_resumen_pago");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.TotalPedido).HasColumnName("total_pedido");
                entity.Property(e => e.TotalPagado).HasColumnName("total_pagado");
                entity.Property(e => e.SaldoPendiente).HasColumnName("saldo_pendiente");
                entity.Property(e => e.IdEstadoFinanciero).HasColumnName("id_estado_financiero");
                entity.Property(e => e.FechaActualizacion).HasColumnName("fecha_actualizacion");

                entity.HasIndex(e => e.IdPedido).IsUnique();

                entity.HasOne(e => e.Pedido)
                    .WithMany()
                    .HasForeignKey(e => e.IdPedido);

                entity.HasOne(e => e.EstadoFinanciero)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstadoFinanciero);
            });

            modelBuilder.Entity<PedidoEstadoFinanciero>(entity =>
            {
                entity.ToTable("pedido_estado_financiero");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<MetodoPago>(entity =>
            {
                entity.ToTable("metodo_pago");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<UnidadMedida>(entity =>
            {
                entity.ToTable("unidad_medida");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Abreviatura).HasColumnName("abreviatura");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<TipoMovimientoInventario>(entity =>
            {
                entity.ToTable("tipo_movimiento_inventario");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.AfectaStock).HasColumnName("afecta_stock");
                entity.Property(e => e.TipoEfecto).HasColumnName("tipo_efecto");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<ProductoTerminado>(entity =>
            {
                entity.ToTable("producto_terminado");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Codigo).HasColumnName("codigo");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.IdTipoPrenda).HasColumnName("id_tipo_prenda");
                entity.Property(e => e.Modelo).HasColumnName("modelo");
                entity.Property(e => e.IdTalla).HasColumnName("id_talla");
                entity.Property(e => e.IdColor).HasColumnName("id_color");
                entity.Property(e => e.IdUnidadMedida).HasColumnName("id_unidad_medida");
                entity.Property(e => e.StockActual).HasColumnName("stock_actual");
                entity.Property(e => e.StockMinimo).HasColumnName("stock_minimo");
                entity.Property(e => e.CostoReferencial).HasColumnName("costo_referencial");
                entity.Property(e => e.PrecioReferencial).HasColumnName("precio_referencial");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasOne(e => e.TipoPrenda).WithMany().HasForeignKey(e => e.IdTipoPrenda);
                entity.HasOne(e => e.Talla).WithMany().HasForeignKey(e => e.IdTalla);
                entity.HasOne(e => e.Color).WithMany().HasForeignKey(e => e.IdColor);
                entity.HasOne(e => e.UnidadMedida).WithMany().HasForeignKey(e => e.IdUnidadMedida);
            });

            modelBuilder.Entity<MovimientoProductoTerminado>(entity =>
            {
                entity.ToTable("movimiento_producto_terminado");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdProductoTerminado).HasColumnName("id_producto_terminado");
                entity.Property(e => e.IdTipoMovimiento).HasColumnName("id_tipo_movimiento");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.StockAnterior).HasColumnName("stock_anterior");
                entity.Property(e => e.StockResultante).HasColumnName("stock_resultante");
                entity.Property(e => e.Motivo).HasColumnName("motivo");
                entity.Property(e => e.Referencia).HasColumnName("referencia");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(e => e.ProductoTerminado).WithMany().HasForeignKey(e => e.IdProductoTerminado);
                entity.HasOne(e => e.TipoMovimiento).WithMany().HasForeignKey(e => e.IdTipoMovimiento);
                entity.HasOne(e => e.Pedido).WithMany().HasForeignKey(e => e.IdPedido);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.IdUsuario);
            });

            modelBuilder.Entity<Insumo>(entity =>
            {
                entity.ToTable("insumo");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Codigo).HasColumnName("codigo");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Categoria).HasColumnName("categoria");
                entity.Property(e => e.IdUnidadMedida).HasColumnName("id_unidad_medida");
                entity.Property(e => e.StockActual).HasColumnName("stock_actual");
                entity.Property(e => e.StockMinimo).HasColumnName("stock_minimo");
                entity.Property(e => e.CostoReferencial).HasColumnName("costo_referencial");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasOne(e => e.UnidadMedida).WithMany().HasForeignKey(e => e.IdUnidadMedida);
            });

            modelBuilder.Entity<MovimientoInsumo>(entity =>
            {
                entity.ToTable("movimiento_insumo");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
                entity.Property(e => e.IdTipoMovimiento).HasColumnName("id_tipo_movimiento");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.StockAnterior).HasColumnName("stock_anterior");
                entity.Property(e => e.StockResultante).HasColumnName("stock_resultante");
                entity.Property(e => e.Motivo).HasColumnName("motivo");
                entity.Property(e => e.Referencia).HasColumnName("referencia");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(e => e.Insumo).WithMany().HasForeignKey(e => e.IdInsumo);
                entity.HasOne(e => e.TipoMovimiento).WithMany().HasForeignKey(e => e.IdTipoMovimiento);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.IdUsuario);
            });

            modelBuilder.Entity<EstadoOrdenProduccion>(entity =>
            {
                entity.ToTable("estado_orden_produccion");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Orden).HasColumnName("orden");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<BOMDetalle>(entity =>
            {
                entity.ToTable("bom_detalle");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdProductoTerminado).HasColumnName("id_producto_terminado");
                entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
                entity.Property(e => e.CantidadRequerida).HasColumnName("cantidad_requerida");
                entity.Property(e => e.IdUnidadMedida).HasColumnName("id_unidad_medida");
                entity.Property(e => e.PorcentajeDesperdicio).HasColumnName("porcentaje_desperdicio");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasIndex(e => new { e.IdProductoTerminado, e.IdInsumo }).IsUnique();

                entity.HasOne(e => e.ProductoTerminado).WithMany().HasForeignKey(e => e.IdProductoTerminado);
                entity.HasOne(e => e.Insumo).WithMany().HasForeignKey(e => e.IdInsumo);
                entity.HasOne(e => e.UnidadMedida).WithMany().HasForeignKey(e => e.IdUnidadMedida);
            });

            modelBuilder.Entity<OrdenProduccion>(entity =>
            {
                entity.ToTable("orden_produccion");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.NumeroOrden).HasColumnName("numero_orden");
                entity.Property(e => e.IdProductoTerminado).HasColumnName("id_producto_terminado");
                entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
                entity.Property(e => e.CantidadProgramada).HasColumnName("cantidad_programada");
                entity.Property(e => e.CantidadProducida).HasColumnName("cantidad_producida");
                entity.Property(e => e.IdEstadoOrdenProduccion).HasColumnName("id_estado_orden_produccion");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
                entity.Property(e => e.IdUsuarioResponsable).HasColumnName("id_usuario_responsable");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.HasIndex(e => e.NumeroOrden).IsUnique();

                entity.HasOne(e => e.ProductoTerminado).WithMany().HasForeignKey(e => e.IdProductoTerminado);
                entity.HasOne(e => e.Pedido).WithMany().HasForeignKey(e => e.IdPedido);
                entity.HasOne(e => e.EstadoOrdenProduccion).WithMany().HasForeignKey(e => e.IdEstadoOrdenProduccion);
                entity.HasOne(e => e.UsuarioResponsable).WithMany().HasForeignKey(e => e.IdUsuarioResponsable);
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("empleado");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
                entity.Property(e => e.Nombres).HasColumnName("nombres");
                entity.Property(e => e.Apellidos).HasColumnName("apellidos");
                entity.Property(e => e.Dpi).HasColumnName("dpi");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
                entity.Property(e => e.Correo).HasColumnName("correo");
                entity.Property(e => e.Direccion).HasColumnName("direccion");
                entity.Property(e => e.Puesto).HasColumnName("puesto");
                entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

                entity.HasOne(e => e.Departamento)
                    .WithMany()
                    .HasForeignKey(e => e.IdDepartamento);
            });

            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.ToTable("asistencia");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.HoraEntrada).HasColumnName("hora_entrada");
                entity.Property(e => e.HoraSalida).HasColumnName("hora_salida");
                entity.Property(e => e.TardanzaMinutos).HasColumnName("tardanza_minutos");
                entity.Property(e => e.HorasTrabajadas).HasColumnName("horas_trabajadas");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

                entity.HasIndex(e => new { e.IdEmpleado, e.Fecha }).IsUnique();

                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado);

                entity.HasOne(e => e.UsuarioRegistro)
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuarioRegistro);
            });

            modelBuilder.Entity<TipoPermiso>(entity =>
            {
                entity.ToTable("tipo_permiso");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Estado).HasColumnName("estado");
            });

            modelBuilder.Entity<PermisoEmpleado>(entity =>
            {
                entity.ToTable("permiso_empleado");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
                entity.Property(e => e.IdTipoPermiso).HasColumnName("id_tipo_permiso");
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
                entity.Property(e => e.Motivo).HasColumnName("motivo");
                entity.Property(e => e.DocumentoUrl).HasColumnName("documento_url");
                entity.Property(e => e.AutorizadoPor).HasColumnName("autorizado_por");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpleado);

                entity.HasOne(e => e.TipoPermiso)
                    .WithMany()
                    .HasForeignKey(e => e.IdTipoPermiso);
            });

            modelBuilder.Entity<Bitacora>(entity =>
            {
                entity.ToTable("bitacora");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Modulo).HasColumnName("modulo");
                entity.Property(e => e.Accion).HasColumnName("accion");
                entity.Property(e => e.TablaAfectada).HasColumnName("tabla_afectada");
                entity.Property(e => e.IdRegistroAfectado).HasColumnName("id_registro_afectado");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.DatosAnteriores).HasColumnName("datos_anteriores");
                entity.Property(e => e.DatosNuevos).HasColumnName("datos_nuevos");
                entity.Property(e => e.IpEquipo).HasColumnName("ip_equipo");
                entity.Property(e => e.FechaEvento).HasColumnName("fecha_evento");

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuario);
            });


        }
    }
}