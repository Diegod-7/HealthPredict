using HealthPredict.Models;
using HealthPredict.Models.FitnessSyncer;
using HealthPredict.Models.HealthAutoExport;
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
        public DbSet<FitnessSyncerConfig> FitnessSyncerConfigs { get; set; }
        public DbSet<HealthAutoExportConfig> HealthAutoExportConfigs { get; set; }

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

                // ✅ NUEVAS PROPIEDADES PARA SISTEMA DE ROLES
                entity.Property(e => e.Rol).HasColumnName("ROL").IsRequired().HasMaxLength(20).HasDefaultValue("Trabajador");
                entity.Property(e => e.Departamento).HasColumnName("DEPARTAMENTO").HasMaxLength(100);
                entity.Property(e => e.Cargo).HasColumnName("CARGO").HasMaxLength(100);
                entity.Property(e => e.JefeId).HasColumnName("JEFE_ID");
                entity.Property(e => e.EsActivo).HasColumnName("ES_ACTIVO").IsRequired().HasDefaultValue(true);

                // ✅ RELACIÓN JEFE-SUBORDINADO (AUTORREFERENCIA)
                entity.HasOne(e => e.Jefe)
                      .WithMany(e => e.Subordinados)
                      .HasForeignKey(e => e.JefeId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_USUARIOS_JEFE");

                // Ignorar propiedades calculadas
                entity.Ignore(e => e.NombreCompleto);
                entity.Ignore(e => e.EsJefe);
                entity.Ignore(e => e.EsTrabajador);
            });

            // Configuración de la tabla DatosVitales
            modelBuilder.Entity<DatoVital>(entity => {
                entity.ToTable("DATOS_VITALES");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID").IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnName("FECHA_REGISTRO").IsRequired();
                entity.Property(e => e.FechaMedicion).HasColumnName("FECHA_MEDICION").IsRequired();
                entity.Property(e => e.TipoDato).HasColumnName("TIPO_DATO").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Valor).HasColumnName("VALOR").IsRequired();
                entity.Property(e => e.Unidad).HasColumnName("UNIDAD").IsRequired().HasMaxLength(20);
                entity.Property(e => e.DispositivoOrigen).HasColumnName("DISPOSITIVO_ORIGEN").HasMaxLength(100);
                entity.Property(e => e.Dispositivo).HasColumnName("DISPOSITIVO").HasMaxLength(100);
                entity.Property(e => e.Fuente).HasColumnName("FUENTE").HasMaxLength(100);
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

            // Configuración de la tabla FitnessSyncerConfigs
            modelBuilder.Entity<FitnessSyncerConfig>(entity => {
                entity.ToTable("FITNESS_SYNCER_CONFIGS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID").IsRequired();
                entity.Property(e => e.AccessToken).HasColumnName("ACCESS_TOKEN").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.RefreshToken).HasColumnName("REFRESH_TOKEN").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.TokenExpiry).HasColumnName("TOKEN_EXPIRY").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE").IsRequired().HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION").IsRequired();
                entity.Property(e => e.UltimaSincronizacion).HasColumnName("ULTIMA_SINCRONIZACION");

                // Relación con Usuario
                entity.HasOne(d => d.Usuario)
                      .WithMany()
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_FITNESS_SYNCER_CONFIGS_USUARIOS");

                // Índice único para evitar múltiples configuraciones activas por usuario
                entity.HasIndex(e => new { e.UsuarioId, e.IsActive })
                      .HasDatabaseName("IX_FITNESS_SYNCER_CONFIGS_USUARIO_ACTIVE")
                      .IsUnique()
                      .HasFilter("IS_ACTIVE = 1");
            });

            // Configuración de la tabla HealthAutoExportConfigs
            modelBuilder.Entity<HealthAutoExportConfig>(entity => {
                entity.ToTable("HEALTH_AUTO_EXPORT_CONFIGS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID").IsRequired();
                entity.Property(e => e.ApiKey).HasColumnName("API_KEY").IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE").IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(e => e.LastSyncAt).HasColumnName("LAST_SYNC_AT");
                entity.Property(e => e.AllowedDataTypes).HasColumnName("ALLOWED_DATA_TYPES").HasMaxLength(1000);
                entity.Property(e => e.DeviceInfo).HasColumnName("DEVICE_INFO").HasMaxLength(500);
                entity.Property(e => e.SyncIntervalMinutes).HasColumnName("SYNC_INTERVAL_MINUTES").IsRequired().HasDefaultValue(60);

                // Relación con Usuario
                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_HEALTH_AUTO_EXPORT_CONFIGS_USUARIOS");

                // Índice único para la API Key
                entity.HasIndex(e => e.ApiKey)
                      .HasDatabaseName("IX_HEALTH_AUTO_EXPORT_CONFIGS_API_KEY")
                      .IsUnique();

                // Índice para búsquedas por usuario
                entity.HasIndex(e => e.UsuarioId)
                      .HasDatabaseName("IX_HEALTH_AUTO_EXPORT_CONFIGS_USUARIO");
            });
        }
    }
} 