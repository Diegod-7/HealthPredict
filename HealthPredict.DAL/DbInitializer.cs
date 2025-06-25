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

            // Verificar si ya hay usuarios
            if (await context.Usuarios.AnyAsync())
            {
                return; // La base de datos ya tiene datos
            }

            // ✅ CREAR USUARIOS DEL SISTEMA DE PERFILAMIENTO

            // 1. CREAR JEFE
            var jefe = new Usuario
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Rodriguez",
                Email = "jefe@healthpredict.com",
                Password = "admin123", // En producción usar hash
                FechaNacimiento = new DateTime(1980, 5, 15),
                Genero = "Masculino",
                Altura = 180,
                Peso = 80.5m,
                FechaRegistro = DateTime.Now,
                UltimoAcceso = DateTime.Now,
                EsProfesionalMedico = false,
                Rol = "Jefe",
                Departamento = "Administración",
                Cargo = "Gerente General",
                JefeId = null, // Es el jefe principal
                EsActivo = true
            };

            // 2. CREAR TRABAJADORES
            var diegoDiaz = new Usuario
            {
                Id = 2,
                Nombre = "Diego",
                Apellido = "Diaz",
                Email = "diego.diaz@healthpredict.com",
                Password = "diego123",
                FechaNacimiento = new DateTime(1995, 3, 15),
                Genero = "Masculino",
                Altura = 175,
                Peso = 75.2m,
                FechaRegistro = new DateTime(2024, 1, 1),
                UltimoAcceso = DateTime.Now,
                EsProfesionalMedico = false,
                Rol = "Trabajador",
                Departamento = "Desarrollo",
                Cargo = "Desarrollador Senior",
                JefeId = 1, // Reporta al jefe
                EsActivo = true
            };

            var matiasMaripangue = new Usuario
            {
                Id = 3,
                Nombre = "Matias",
                Apellido = "Maripangue",
                Email = "matias.maripangue@healthpredict.com",
                Password = "matias123",
                FechaNacimiento = new DateTime(1992, 8, 22),
                Genero = "Masculino",
                Altura = 178,
                Peso = 82.3m,
                FechaRegistro = new DateTime(2024, 1, 15),
                UltimoAcceso = DateTime.Now,
                EsProfesionalMedico = false,
                Rol = "Trabajador",
                Departamento = "Desarrollo",
                Cargo = "Desarrollador Full Stack",
                JefeId = 1, // Reporta al jefe
                EsActivo = true
            };

            var iahnVera = new Usuario
            {
                Id = 4,
                Nombre = "Iahn",
                Apellido = "Vera",
                Email = "iahn.vera@healthpredict.com",
                Password = "iahn123",
                FechaNacimiento = new DateTime(1994, 11, 10),
                Genero = "Masculino",
                Altura = 172,
                Peso = 68.7m,
                FechaRegistro = new DateTime(2024, 2, 1),
                UltimoAcceso = DateTime.Now,
                EsProfesionalMedico = false,
                Rol = "Trabajador",
                Departamento = "Desarrollo",
                Cargo = "Desarrollador Frontend",
                JefeId = 1, // Reporta al jefe
                EsActivo = true
            };

            // Agregar usuarios al contexto
            await context.Usuarios.AddRangeAsync(jefe, diegoDiaz, matiasMaripangue, iahnVera);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Base de datos inicializada con usuarios del sistema de perfilamiento:");
            Console.WriteLine($"   👔 Jefe: {jefe.NombreCompleto} ({jefe.Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 1: {diegoDiaz.NombreCompleto} ({diegoDiaz.Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 2: {matiasMaripangue.NombreCompleto} ({matiasMaripangue.Email})");
            Console.WriteLine($"   👨‍💻 Trabajador 3: {iahnVera.NombreCompleto} ({iahnVera.Email})");
        }
    }
} 