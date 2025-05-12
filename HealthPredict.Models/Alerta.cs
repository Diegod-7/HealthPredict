using System;

namespace HealthPredict.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoAlerta { get; set; }
        public string Descripcion { get; set; }
        public string Severidad { get; set; }
        public bool Leida { get; set; }
        public DateTime? FechaLectura { get; set; }
        public bool Resuelta { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? NotasResolucion { get; set; }

        // Relaciones
        public virtual Usuario? Usuario { get; set; }
    }
} 