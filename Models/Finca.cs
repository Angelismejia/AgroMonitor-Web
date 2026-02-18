// Models/Finca.cs
// ============================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Finca
    {
        [Key]
        public int IdFinca { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre de la finca es obligatorio")]
        [StringLength(60)]
        public string NombreFinca { get; set; } = string.Empty;

        [StringLength(150)]
        public string? UbicacionText { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitud { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitud { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AreaTareas { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.Now;

        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        public virtual ICollection<Cultivo> Cultivos { get; set; } = new List<Cultivo>();
        public virtual ICollection<Sensor> Sensores { get; set; } = new List<Sensor>();
        public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
        public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    }
}