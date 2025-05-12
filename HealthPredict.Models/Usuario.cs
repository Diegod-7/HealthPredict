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

        // Relaciones
        public virtual ICollection<DatoVital>? DatosVitales { get; set; }
        public virtual ICollection<Alerta>? Alertas { get; set; }
    }
} 