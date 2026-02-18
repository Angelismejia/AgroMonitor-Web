// ============================================
// Models/Alerta.cs
// ============================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Alerta
    {
        [Key]
        public int IdAlerta { get; set; }

        [Required]
        public int IdCultivo { get; set; }

        [Required]
        public int IdFinca { get; set; }

        [Required]
        public string TipoAlerta { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string NivelPrioridad { get; set; } = "media";

        public DateTime FechaGenerada { get; set; } = DateTime.Now;

        [Required]
        public string Estado { get; set; } = "pendiente";

        [ForeignKey("IdCultivo")]
        public virtual Cultivo? Cultivo { get; set; }

        [ForeignKey("IdFinca")]
        public virtual Finca? Finca { get; set; }
    }
}