using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//[Table("PagoDetalle")]
public class Saldo
{
    [Key]
    public int SaldoID { get; set; }

    [Required]
    public int ClienteID { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    [Required]
    public DateTime FechaCarga { get; set; }

}