using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Pagos")]
public class Pago
{
    [Key]
    public int PagoID { get; set; }

    [Required]
    public int ClienteID { get; set; }

    [Required]
    public int PaqueteID { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPagar { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }

    [Required]
    [MaxLength(10)]
    public string Estado { get; set; }
}