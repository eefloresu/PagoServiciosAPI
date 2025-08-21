using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagoServiciosAPI.Models
{
    public class Clientes
    {
        [Column("ClienteId")]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Telefono { get; set; }

        // Constructor por defecto

    }
}