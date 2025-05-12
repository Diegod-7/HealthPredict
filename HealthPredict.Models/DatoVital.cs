using System;

namespace HealthPredict.Models
{
    public class DatoVital
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDato { get; set; }
        public decimal Valor { get; set; }
        public string Unidad { get; set; }
        public string? DispositivoOrigen { get; set; }
        public string? Notas { get; set; }

        // Relaciones
        public virtual Usuario? Usuario { get; set; }
    }
} 