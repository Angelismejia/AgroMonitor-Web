// ============================================
// Models/Sensor.cs
// ============================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Sensor
    {
        [Key]
        public int IdSensor { get; set; }

        [Required]
        public int IdFinca { get; set; }

        [Required(ErrorMessage = "El identificador del sensor es obligatorio")]
        [StringLength(80)]
        public string IdentificadorExterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de sensor es obligatorio")]
        public string TipoSensor { get; set; } = "humedad";

        [StringLength(40)]
        public string? Modelo { get; set; }

        [StringLength(100)]
        public string? UbicacionEnFinca { get; set; }

        [Required]
        public string Estado { get; set; } = "activo";

        public DateTime InstaladoEn { get; set; } = DateTime.Now;

        public DateTime? UltimoCheck { get; set; }

        [ForeignKey("IdFinca")]
        public virtual Finca? Finca { get; set; }

        public virtual ICollection<LecturaSensor> Lecturas { get; set; } = new List<LecturaSensor>();
    }
}