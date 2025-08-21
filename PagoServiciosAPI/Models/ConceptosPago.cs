using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagoServiciosAPI.Models
{
    public class ConceptosPago
    {
        [Column("ConceptoID")]
        public int Id { get; set; }
        [Required]
        public string NombreConcepto { get; set; }

        // Constructor por defecto
    }
}