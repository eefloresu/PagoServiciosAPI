using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagoServiciosAPI.Models
{
    public class Paquete
    {
        [Column("PaqueteID")]
        public int Id { get; set; }
        [Required]
        public string NombrePaquete { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public decimal Precio { get; set; }

        // Constructor por defecto
    }
}