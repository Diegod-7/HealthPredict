using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.DAL
{
    public static class DbInitializer
    {
        public static async Task Initialize(HealthPredictContext context)
        {
            // Asegurarse que la base de datos está creada
            await context.Database.MigrateAsync();

            // Verificar si ya hay datos en la base de datos
            if (await context.Usuarios.AnyAsync())
            {
                return; // La base de datos ya tiene datos
            }

            // Crear usuarios de prueba
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan.perez@example.com",
                    Password = "password123",
                    FechaNacimiento = new DateTime(1980, 5, 15),
                    Genero = "Masculino",
                    Altura = 1.75m,
                    Peso = 75.5m,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                },
                new Usuario
                {
                    Nombre = "María",
                    Apellido = "González",
                    Email = "maria.gonzalez@example.com",
                    Password = "password123",
                    FechaNacimiento = new DateTime(1985, 8, 22),
                    Genero = "Femenino",
                    Altura = 1.65m,
                    Peso = 62.3m,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                },
                new Usuario
                {
                    Nombre = "Carlos",
                    Apellido = "Rodríguez",
                    Email = "carlos.rodriguez@example.com",
                    Password = "password123",
                    FechaNacimiento = new DateTime(1975, 3, 10),
                    Genero = "Masculino",
                    Altura = 1.80m,
                    Peso = 85.0m,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = true,
                    Especialidad = "Cardiología",
                    NumeroLicencia = "MED12345"
                }
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            // Crear datos vitales de prueba
            var random = new Random();
            var datosVitales = new List<DatoVital>();

            foreach (var usuario in usuarios.Where(u => !u.EsProfesionalMedico))
            {
                // Datos de frecuencia cardíaca
                for (int i = 0; i < 10; i++)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuario.Id,
                        FechaRegistro = DateTime.Now.AddDays(-i),
                        TipoDato = "Frecuencia Cardíaca",
                        Valor = random.Next(60, 100),
                        Unidad = "ppm",
                        DispositivoOrigen = "Smartwatch",
                        Notas = "Medición en reposo"
                    });
                }

                // Datos de presión arterial
                for (int i = 0; i < 10; i++)
                {
                    int sistolica = random.Next(110, 140);
                    int diastolica = random.Next(70, 90);
                    
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuario.Id,
                        FechaRegistro = DateTime.Now.AddDays(-i),
                        TipoDato = "Presión Arterial Sistólica",
                        Valor = sistolica,
                        Unidad = "mmHg",
                        DispositivoOrigen = "Tensiómetro Digital",
                        Notas = $"Sistólica: {sistolica}, Diastólica: {diastolica}"
                    });
                    
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuario.Id,
                        FechaRegistro = DateTime.Now.AddDays(-i),
                        TipoDato = "Presión Arterial Diastólica",
                        Valor = diastolica,
                        Unidad = "mmHg",
                        DispositivoOrigen = "Tensiómetro Digital",
                        Notas = $"Sistólica: {sistolica}, Diastólica: {diastolica}"
                    });
                }

                // Datos de glucosa
                for (int i = 0; i < 10; i++)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuario.Id,
                        FechaRegistro = DateTime.Now.AddDays(-i),
                        TipoDato = "Nivel de Glucosa",
                        Valor = random.Next(80, 130),
                        Unidad = "mg/dL",
                        DispositivoOrigen = "Glucómetro",
                        Notas = "Medición antes de comida"
                    });
                }
            }

            await context.DatosVitales.AddRangeAsync(datosVitales);
            await context.SaveChangesAsync();

            // Crear alertas de prueba
            var alertas = new List<Alerta>();
            var tiposAlerta = new[] { "Frecuencia Cardíaca Alta", "Presión Arterial Alta", "Nivel de Glucosa Alto" };
            var severidades = new[] { "Baja", "Media", "Alta" };

            foreach (var usuario in usuarios.Where(u => !u.EsProfesionalMedico))
            {
                for (int i = 0; i < 3; i++)
                {
                    var tipoAlerta = tiposAlerta[random.Next(tiposAlerta.Length)];
                    var severidad = severidades[random.Next(severidades.Length)];
                    
                    alertas.Add(new Alerta
                    {
                        UsuarioId = usuario.Id,
                        FechaCreacion = DateTime.Now.AddDays(-random.Next(1, 30)),
                        TipoAlerta = tipoAlerta,
                        Descripcion = $"Se ha detectado un nivel {tipoAlerta.ToLower()} que requiere atención",
                        Severidad = severidad,
                        Leida = random.Next(2) == 0,
                        FechaLectura = random.Next(2) == 0 ? DateTime.Now.AddDays(-random.Next(1, 10)) : (DateTime?)null,
                        Resuelta = random.Next(2) == 0,
                        FechaResolucion = random.Next(2) == 0 ? DateTime.Now.AddDays(-random.Next(1, 5)) : (DateTime?)null,
                        NotasResolucion = random.Next(2) == 0 ? "Se realizó seguimiento médico" : null
                    });
                }
            }

            await context.Alertas.AddRangeAsync(alertas);
            await context.SaveChangesAsync();
        }
    }
} 