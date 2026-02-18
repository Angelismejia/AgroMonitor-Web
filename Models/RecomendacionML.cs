// ============================================
// Models/RecomendacionML.cs
// ============================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class RecomendacionML
    {
        [Key]
        public int IdRec { get; set; }

        [Required]
        public int IdCultivo { get; set; }

        [Required]
        public string TipoRecomendacion { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Confianza { get; set; } = 0;

        public DateTime FechaGenerada { get; set; } = DateTime.Now;

        [ForeignKey("IdCultivo")]
        public virtual Cultivo? Cultivo { get; set; }
    }
}


