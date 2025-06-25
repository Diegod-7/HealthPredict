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
        public static async Task InitializeAsync(HealthPredictContext context)
        {
            // Asegurar que la base de datos esté creada
            await context.Database.EnsureCreatedAsync();

            // Verificar si ya existen usuarios
            if (context.Usuarios.Any())
            {
                return; // Los datos ya están inicializados
            }

            // Crear usuarios predefinidos
            var usuarios = new List<Usuario>
            {
                // Jefe
                new Usuario
                {
                    Nombre = "Carlos",
                    Apellido = "Rodriguez",
                    Email = "carlos.rodriguez@healthpredict.com",
                    Password = "admin123",
                    FechaNacimiento = new DateTime(1985, 3, 15),
                    Genero = "Masculino",
                    Altura = 178,
                    Peso = 80.0m,
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    EsProfesionalMedico = false,
                    Rol = "Jefe",
                    Departamento = "Administración",
                    Cargo = "Gerente General",
                    JefeId = null,
                    EsActivo = true
                },
                
                // Trabajadores
                new Usuario
                {
                    Nombre = "Diego",
                    Apellido = "Diaz",
                    Email = "diego.diaz@healthpredict.com",
                    Password = "diego123",
                    FechaNacimiento = new DateTime(1992, 8, 22),
                    Genero = "Masculino",
                    Altura = 175,
                    Peso = 75.0m,
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    EsProfesionalMedico = false,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador Full Stack",
                    JefeId = 1, // Carlos Rodriguez será ID 1
                    EsActivo = true
                },
                
                new Usuario
                {
                    Nombre = "Iahn",
                    Apellido = "Vera",
                    Email = "iahn.vera@healthpredict.com",
                    Password = "iahn123",
                    FechaNacimiento = new DateTime(1994, 11, 10),
                    Genero = "Masculino",
                    Altura = 172,
                    Peso = 70.0m,
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    EsProfesionalMedico = false,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador Frontend",
                    JefeId = 1, // Carlos Rodriguez será ID 1
                    EsActivo = true
                },
                
                new Usuario
                {
                    Nombre = "Matias",
                    Apellido = "Maripangue",
                    Email = "matias.maripangue@healthpredict.com",
                    Password = "matias123",
                    FechaNacimiento = new DateTime(1993, 6, 5),
                    Genero = "Masculino",
                    Altura = 180,
                    Peso = 82.0m,
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    EsProfesionalMedico = false,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador Backend",
                    JefeId = 1, // Carlos Rodriguez será ID 1
                    EsActivo = true
                }
            };

            context.Usuarios.AddRange(usuarios);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Base de datos inicializada con usuarios del sistema de perfilamiento:");
            Console.WriteLine($"   👔 Jefe: {usuarios[0].NombreCompleto} ({usuarios[0].Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 1: {usuarios[1].NombreCompleto} ({usuarios[1].Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 2: {usuarios[2].NombreCompleto} ({usuarios[2].Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 3: {usuarios[3].NombreCompleto} ({usuarios[3].Email})");
        }
    }
} 