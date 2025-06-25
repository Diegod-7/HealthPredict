using System;
using System.Collections.Generic;

namespace HealthPredict.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public decimal Altura { get; set; }
        public decimal Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public bool EsProfesionalMedico { get; set; }
        public string? Especialidad { get; set; }
        public string? NumeroLicencia { get; set; }

        // ✅ SISTEMA DE ROLES Y PERFILES
        public string Rol { get; set; } = "Trabajador"; // "Jefe" o "Trabajador"
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
        public int? JefeId { get; set; } // ID del jefe si es trabajador
        public bool EsActivo { get; set; } = true;

        // Relación con el jefe
        public virtual Usuario? Jefe { get; set; }
        public virtual ICollection<Usuario>? Subordinados { get; set; }

        // Relaciones existentes
        public virtual ICollection<DatoVital>? DatosVitales { get; set; }
        public virtual ICollection<Alerta>? Alertas { get; set; }

        // ✅ PROPIEDADES CALCULADAS
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public bool EsJefe => Rol == "Jefe";
        public bool EsTrabajador => Rol == "Trabajador";
    }
} 