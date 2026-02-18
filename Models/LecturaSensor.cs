using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class LecturaSensor
    {
        [Key]
        public long IdLectura { get; set; }

        [Required]
        public int IdSensor { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,4)")]
        public decimal Valor { get; set; }

        [Required]
        [StringLength(10)]
        public string Unidad { get; set; } = string.Empty;

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        public bool ProcesadoML { get; set; } = false;

        [ForeignKey("IdSensor")]
        public virtual Sensor? Sensor { get; set; }
    }
}