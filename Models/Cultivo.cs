// ============================================
// Models/Cultivo.cs
// ============================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Cultivo
    {
        [Key]
        public int IdCultivo { get; set; }

        [Required]
        public int IdFinca { get; set; }

        [Required(ErrorMessage = "El tipo de cultivo es obligatorio")]
        public string TipoCultivo { get; set; } = "arroz";

        [Required(ErrorMessage = "La fecha de siembra es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaSiembra { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaCosechaEstimada { get; set; }

        [Required]
        public string Estado { get; set; } = "activo";

        [ForeignKey("IdFinca")]
        public virtual Finca? Finca { get; set; }

        public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
        public virtual ICollection<RecomendacionML> Recomendaciones { get; set; } = new List<RecomendacionML>();
    }
}