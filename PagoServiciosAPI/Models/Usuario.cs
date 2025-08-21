using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagoServiciosAPI.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Clave { get; set; }

        [Required]
        public string role { get; set; }
    }
}