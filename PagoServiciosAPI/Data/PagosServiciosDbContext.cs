using Microsoft.EntityFrameworkCore;
using PagoServiciosAPI.Models;


namespace PagoServiciosAPI.Data
{
    public class PagosServiciosDbContext : DbContext
    {
        public PagosServiciosDbContext(DbContextOptions<PagosServiciosDbContext> options) : base(options)
        {
        }

        //public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<ConceptosPago> ConceptosPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<PagoDetalle> PagoDetalle { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}