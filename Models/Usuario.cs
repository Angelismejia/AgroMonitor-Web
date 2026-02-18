// ============================================
// Models/Usuario.cs
// ============================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(80)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        public string ContraseñaHash { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "productor";

        [Phone]
        [StringLength(15)]
        public string? Telefono { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.Now;

        public virtual ICollection<Finca> Fincas { get; set; } = new List<Finca>();
        public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    }
}
