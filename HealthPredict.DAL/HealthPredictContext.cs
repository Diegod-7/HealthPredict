using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPredict.DAL
{
    public class HealthPredictContext : DbContext
    {
        public HealthPredictContext(DbContextOptions<HealthPredictContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<DatoVital> DatosVitales { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla Usuarios
            modelBuilder.Entity<Usuario>(entity => {
                entity.ToTable("USUARIOS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).HasColumnName("APELLIDO").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("EMAIL").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).HasColumnName("PASSWORD").IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaNacimiento).HasColumnName("FECHA_NACIMIENTO").IsRequired();
                entity.Property(e => e.Genero).HasColumnName("GENERO").IsRequired().HasMaxLength(20);
                entity.Property(e => e.Altura).HasColumnName("ALTURA").IsRequired();
                entity.Property(e => e.Peso).HasColumnName("PESO").IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnName("FECHA_REGISTRO").IsRequired();
                entity.Property(e => e.UltimoAcceso).HasColumnName("ULTIMO_ACCESO").IsRequired();
                entity.Property(e => e.EsProfesionalMedico).HasColumnName("ES_PROFESIONAL_MEDICO").IsRequired();
                entity.Property(e => e.Especialidad).HasColumnName("ESPECIALIDAD").HasMaxLength(100);
                entity.Property(e => e.NumeroLicencia).HasColumnName("NUMERO_LICENCIA").HasMaxLength(50);
            });

            // Configuración de la tabla DatosVitales
            modelBuilder.Entity<DatoVital>(entity => {
                entity.ToTable("DATOS_VITALES");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID").IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnName("FECHA_REGISTRO").IsRequired();
                entity.Property(e => e.TipoDato).HasColumnName("TIPO_DATO").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Valor).HasColumnName("VALOR").IsRequired();
                entity.Property(e => e.Unidad).HasColumnName("UNIDAD").IsRequired().HasMaxLength(20);
                entity.Property(e => e.DispositivoOrigen).HasColumnName("DISPOSITIVO_ORIGEN").HasMaxLength(100);
                entity.Property(e => e.Notas).HasColumnName("NOTAS").HasMaxLength(500);

                // Relación con Usuario
                entity.HasOne(d => d.Usuario)
                      .WithMany(p => p.DatosVitales)
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_DATOS_VITALES_USUARIOS");
            });

            // Configuración de la tabla Alertas
            modelBuilder.Entity<Alerta>(entity => {
                entity.ToTable("ALERTAS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID").IsRequired();
                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION").IsRequired();
                entity.Property(e => e.TipoAlerta).HasColumnName("TIPO_ALERTA").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasColumnName("DESCRIPCION").IsRequired().HasMaxLength(500);
                entity.Property(e => e.Severidad).HasColumnName("SEVERIDAD").IsRequired().HasMaxLength(20);
                entity.Property(e => e.Leida).HasColumnName("LEIDA").IsRequired();
                entity.Property(e => e.FechaLectura).HasColumnName("FECHA_LECTURA");
                entity.Property(e => e.Resuelta).HasColumnName("RESUELTA").IsRequired();
                entity.Property(e => e.FechaResolucion).HasColumnName("FECHA_RESOLUCION");
                entity.Property(e => e.NotasResolucion).HasColumnName("NOTAS_RESOLUCION").HasMaxLength(500);

                // Relación con Usuario
                entity.HasOne(d => d.Usuario)
                      .WithMany(p => p.Alertas)
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ALERTAS_USUARIOS");
            });
        }
    }
} 