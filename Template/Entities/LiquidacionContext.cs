using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Template.Entities
{
    public partial class LiquidacionContext : DbContext
    {
        public LiquidacionContext()
        {
        }

        public LiquidacionContext(DbContextOptions<LiquidacionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Empleado> Empleados { get; set; } = null!;
        public virtual DbSet<Empresa> Empresas { get; set; } = null!;
        public virtual DbSet<LiquiCodLiquidado> LiquiCodLiquidados { get; set; } = null!;
        public virtual DbSet<Liquidacion> Liquidacions { get; set; } = null!;
        public virtual DbSet<TipoLiquidacion> TipoLiquidacions { get; set; } = null!;
        public virtual DbSet<TokenRecuperacion> TokenRecuperacions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleado");

                entity.Property(e => e.Id)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Apellido)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Dvh).HasColumnName("DVH");

                entity.Property(e => e.EmpresaId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("Empresa_Id");

                entity.Property(e => e.Firma).HasColumnType("image");

                entity.Property(e => e.Ingreso).HasColumnType("datetime");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Empresa)
                    .WithMany(p => p.Empleados)
                    .HasForeignKey(d => d.EmpresaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Empresa_Empleado_FK");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresa");

                entity.Property(e => e.Id)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Dvh).HasColumnName("DVH");

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Responsable)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LiquiCodLiquidado>(entity =>
            {
                entity.HasKey(e => new { e.LiquiId, e.EmpleadoId, e.Codigo })
                    .HasName("PK_Liqui_Empleado_Codigo");

                entity.ToTable("LiquiCodLiquidado");

                entity.Property(e => e.LiquiId).HasColumnName("Liqui_Id");

                entity.Property(e => e.EmpleadoId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("Empleado_Id");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CodDescripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodTipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dvh).HasColumnName("DVH");

                entity.Property(e => e.Importe).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Porcentaje).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Liquidacion)
                    .WithMany(p => p.LiquiCodLiquidados)
                    .HasForeignKey(d => new { d.LiquiId, d.EmpleadoId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Empleado_Liquidacion_en_LiquiCodLiquidado");
            });

            modelBuilder.Entity<Liquidacion>(entity =>
            {
                entity.HasKey(e => new { e.LiquiId, e.EmpleadoId })
                    .HasName("PK_Liqui_Empleado");

                entity.ToTable("Liquidacion");

                entity.Property(e => e.LiquiId).HasColumnName("Liqui_Id");

                entity.Property(e => e.EmpleadoId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("Empleado_Id");

                entity.Property(e => e.Banco)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Categoria)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dvh).HasColumnName("DVH");

                entity.Property(e => e.FecUltDeposito).HasColumnType("datetime");

                entity.Property(e => e.Firmado).HasColumnType("datetime");

                entity.Property(e => e.ReciboNro).ValueGeneratedOnAdd();

                entity.Property(e => e.SueldoBasico).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.TotalDeducciones).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.TotalHaberes).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.TotalNeto).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Empleado)
                    .WithMany(p => p.Liquidacions)
                    .HasForeignKey(d => d.EmpleadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Empleado_Liquidacion");
            });

            modelBuilder.Entity<TipoLiquidacion>(entity =>
            {
                entity.ToTable("TipoLiquidacion");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TokenRecuperacion>(entity =>
            {
                entity.ToTable("TokenRecuperacion");

                entity.Property(e => e.EmpleadoId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("Empleado_Id");

                entity.Property(e => e.Expitarion).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Empleado)
                    .WithMany(p => p.TokenRecuperacions)
                    .HasForeignKey(d => d.EmpleadoId)
                    .HasConstraintName("TokenRecuperacion_Empleado_FK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
