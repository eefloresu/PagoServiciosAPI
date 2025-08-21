using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//[Table("PagoDetalle")]
public class PagoDetalle
{
    [Key]
    public int PagoDetalleID { get; set; }

    [Required]
    public int PagoID { get; set; }

    [Required]
    public int ConceptoID { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

}