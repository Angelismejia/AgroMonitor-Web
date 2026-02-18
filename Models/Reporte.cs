// ============================================
// Models/Reporte.cs
// ============================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Reporte
    {
        [Key]
        public int IdReporte { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdFinca { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Contenido { get; set; } = string.Empty;

        public DateTime FechaGenerado { get; set; } = DateTime.Now;

        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("IdFinca")]
        public virtual Finca? Finca { get; set; }
    }
}