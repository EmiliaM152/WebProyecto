using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ReactVentas.Models
{
    // Contexto de la base de datos.
    public partial class DBREACT_VENTAContext : DbContext
    {
        public DBREACT_VENTAContext()
        {
            // Constructor sin parámetros para el contexto de la base de datos.
        }

        // Constructor que recibe opciones de configuración para el contexto de la base de datos.
        public DBREACT_VENTAContext(DbContextOptions<DBREACT_VENTAContext> options)
            : base(options)
        {

        }

        // DbSet que representan las tablas en la base de datos.
        public virtual DbSet<Categoria> Categoria { get; set; } = null!;
        public virtual DbSet<DetalleVenta> DetalleVenta { get; set; } = null!;
        public virtual DbSet<NumeroDocumento> NumeroDocumentos { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Venta> Venta { get; set; } = null!;

        // Método para configurar opciones adicionales del contexto de la base de datos.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        // Método para configurar el modelo de la base de datos.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de las entidades (tablas) en la base de datos.

            // Configuración de la entidad 'Categoria'
            modelBuilder.Entity<Categoria>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdCategoria)
                    .HasName("PK__Categori__8A3D240CED1907F1");

                // Configuración de las propiedades de la entidad 'Categoria'.
                entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.EsActivo).HasColumnName("esActivo");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaRegistro")
                    .HasDefaultValueSql("(getdate())");
            });

            // Configuración de la entidad 'DetalleVenta'
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdDetalleVenta)
                    .HasName("PK__DetalleV__BFE2843F851DE491");

                // Configuración de las propiedades de la entidad 'DetalleVenta'.
                entity.Property(e => e.IdDetalleVenta).HasColumnName("idDetalleVenta");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.IdProducto).HasColumnName("idProducto");
                entity.Property(e => e.IdVenta).HasColumnName("idVenta");

                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("precio");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total");

                // Definición de las relaciones con otras entidades.
                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.DetalleVenta)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK__DetalleVe__idPro__60A75C0F");

                entity.HasOne(d => d.IdVentaNavigation)
                    .WithMany(p => p.DetalleVenta)
                    .HasForeignKey(d => d.IdVenta)
                    .HasConstraintName("FK__DetalleVe__idVen__5FB337D6");
            });

            // Configuración de la entidad 'NumeroDocumento'
            modelBuilder.Entity<NumeroDocumento>(entity =>
            {
                // Mapeo a la tabla 'NumeroDocumento'
                entity.ToTable("NumeroDocumento");

                // Definición de la clave primaria y propiedad autoincremental
                entity.Property(e => e.Id)
                    .ValueGeneratedNever() // No es autoincremental
                    .HasColumnName("id");

                // Configuración de la propiedad 'FechaRegistro'
                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaRegistro")
                    .HasDefaultValueSql("(getdate())");
            });

            // Configuración de la entidad 'Producto'
            modelBuilder.Entity<Producto>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdProducto)
                    .HasName("PK__Producto__07F4A132B3DE1441");

                // Mapeo a la tabla 'Producto'
                entity.ToTable("Producto");

                // Configuración de las propiedades de la entidad 'Producto'.
                entity.Property(e => e.IdProducto).HasColumnName("idProducto");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.EsActivo).HasColumnName("esActivo");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaRegistro")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");

                entity.Property(e => e.Marca)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("marca");

                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("precio");

                entity.Property(e => e.Stock).HasColumnName("stock");

                // Definición de la relación con la entidad 'Categoria'
                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdCategoria)
                    .HasConstraintName("FK__Producto__idCate__5812160E");
            });

            // Configuración de la entidad 'Rol'
            modelBuilder.Entity<Rol>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdRol)
                    .HasName("PK__Rol__3C872F76D60D3150");

                // Mapeo a la tabla 'Rol'
                entity.ToTable("Rol");

                // Configuración de las propiedades de la entidad 'Rol'.
                entity.Property(e => e.IdRol).HasColumnName("idRol");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.EsActivo).HasColumnName("esActivo");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaRegistro")
                    .HasDefaultValueSql("(getdate())");
            });

            // Configuración de la entidad 'Usuario'
            modelBuilder.Entity<Usuario>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__Usuario__645723A6E137D226");

                // Mapeo a la tabla 'Usuario'
                entity.ToTable("Usuario");

                // Configuración de las propiedades de la entidad 'Usuario'.
                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.Clave)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("clave");

                entity.Property(e => e.Correo)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("correo");

                entity.Property(e => e.EsActivo).HasColumnName("esActivo");

                entity.Property(e => e.IdRol).HasColumnName("idRol");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("telefono");

                // Definición de la relación con la entidad 'Rol'
                entity.HasOne(d => d.IdRolNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdRol)
                    .HasConstraintName("FK__Usuario__idRol__3A81B327");
            });

            // Configuración de la entidad 'Venta'
            modelBuilder.Entity<Venta>(entity =>
            {
                // Definición de la clave primaria y nombre de la restricción.
                entity.HasKey(e => e.IdVenta)
                    .HasName("PK__Venta__077D5614D2880592");

                // Mapeo a la tabla 'Venta'
                entity.Property(e => e.IdVenta).HasColumnName("idVenta");

                // Configuración de las propiedades de la entidad 'Venta'.
                entity.Property(e => e.DocumentoCliente)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("documentoCliente");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaRegistro")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.ImpuestoTotal)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("impuestoTotal");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("nombreCliente");

                entity.Property(e => e.NumeroDocumento)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("numeroDocumento");

                entity.Property(e => e.SubTotal)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("subTotal");

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tipoDocumento");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total");

                // Definición de la relación con la entidad 'Usuario'
                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Venta)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK__Venta__idUsuario__5CD6CB2B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}