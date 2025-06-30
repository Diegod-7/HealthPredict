using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPredict.DAL
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(HealthPredictContext context)
        {
            try
            {
                // Verificar si ya hay datos
                if (await context.DatosVitales.AnyAsync() || await context.Alertas.AnyAsync())
                {
                    Console.WriteLine("✅ Los datos ya existen, omitiendo seeding");
                    return;
                }

                Console.WriteLine("🌱 Iniciando seeding de datos...");

                // Obtener usuarios existentes
                var usuarios = await context.Usuarios.ToListAsync();
                if (!usuarios.Any())
                {
                    Console.WriteLine("❌ No hay usuarios para generar datos");
                    return;
                }

                Console.WriteLine($"📊 Generando datos para {usuarios.Count} usuarios...");

                var random = new Random(42); // Seed fijo para datos consistentes
                var fechaInicio = DateTime.Now.AddDays(-30); // Solo últimos 30 días para empezar

                // 1. GENERAR DATOS VITALES SIMPLES
                var datosVitales = new List<DatoVital>();
                
                foreach (var usuario in usuarios)
                {
                    Console.WriteLine($"   Generando datos para {usuario.Nombre}...");
                    
                    // Generar 30 días de datos básicos
                    for (int dia = 0; dia < 30; dia++)
                    {
                        var fecha = fechaInicio.AddDays(dia);
                        
                        // Solo datos básicos por ahora
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuario.Id,
                            TipoDato = "Presión Sistólica",
                            Valor = random.Next(110, 160),
                            Unidad = "mmHg",
                            FechaRegistro = fecha.AddHours(8),
                            DispositivoOrigen = "Manual"
                        });

                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuario.Id,
                            TipoDato = "Frecuencia Cardíaca",
                            Valor = random.Next(60, 100),
                            Unidad = "bpm",
                            FechaRegistro = fecha.AddHours(9),
                            DispositivoOrigen = "Smartwatch"
                        });

                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuario.Id,
                            TipoDato = "Pasos",
                            Valor = random.Next(3000, 12000),
                            Unidad = "pasos",
                            FechaRegistro = fecha.AddHours(23),
                            DispositivoOrigen = "Smartphone"
                        });
                    }
                }

                Console.WriteLine($"💾 Guardando {datosVitales.Count} datos vitales...");
                await context.DatosVitales.AddRangeAsync(datosVitales);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Generados {datosVitales.Count} datos vitales");

                // 2. GENERAR ALERTAS SIMPLES
                Console.WriteLine("🚨 Generando alertas...");
                var alertas = new List<Alerta>();
                
                // Generar alertas básicas para cada usuario
                foreach (var usuario in usuarios)
                {
                    // Alertas por presión alta (basadas en datos generados)
                    var presionesAltas = datosVitales
                        .Where(d => d.UsuarioId == usuario.Id && d.TipoDato == "Presión Sistólica" && d.Valor > 140)
                        .Take(2);

                    foreach (var presion in presionesAltas)
                    {
                        alertas.Add(new Alerta
                        {
                            UsuarioId = usuario.Id,
                            TipoAlerta = "Presión Arterial Alta",
                            Severidad = presion.Valor > 160 ? "Crítica" : "Alta",
                            Descripcion = $"Presión sistólica de {presion.Valor} mmHg detectada",
                            FechaCreacion = presion.FechaRegistro.AddMinutes(30),
                            Leida = random.Next(0, 100) < 70,
                            Resuelta = random.Next(0, 100) < 30
                        });
                    }

                    // Alertas por actividad baja
                    var actividadBaja = datosVitales
                        .Where(d => d.UsuarioId == usuario.Id && d.TipoDato == "Pasos" && d.Valor < 5000)
                        .Take(1);

                    foreach (var pasos in actividadBaja)
                    {
                        alertas.Add(new Alerta
                        {
                            UsuarioId = usuario.Id,
                            TipoAlerta = "Actividad Física Baja",
                            Severidad = "Media",
                            Descripcion = $"Solo {pasos.Valor} pasos registrados",
                            FechaCreacion = pasos.FechaRegistro.AddHours(1),
                            Leida = random.Next(0, 100) < 60,
                            Resuelta = random.Next(0, 100) < 40
                        });
                    }
                }

                Console.WriteLine($"💾 Guardando {alertas.Count} alertas...");
                await context.Alertas.AddRangeAsync(alertas);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Generadas {alertas.Count} alertas");

                Console.WriteLine("🎉 Seeding completado exitosamente!");
                Console.WriteLine($"📊 Resumen:");
                Console.WriteLine($"   • {datosVitales.Count} datos vitales");
                Console.WriteLine($"   • {alertas.Count} alertas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en seeding: {ex.Message}");
                Console.WriteLine($"📄 Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 