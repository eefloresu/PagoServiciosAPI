using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagoServiciosAPI.Data;


namespace PagoServiciosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoDetallesController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<PagoDetallesController> _logger;

        public PagoDetallesController(PagosServiciosDbContext context, ILogger<PagoDetallesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<PagoDetalle>>> Listar()
        {
            try
            {
                var pagoDetalle = await _context.PagoDetalle.ToListAsync();
                if (pagoDetalle == null || !pagoDetalle.Any())
                {
                    _logger.LogWarning("No se encontraron Detalles del pago.");
                    return NotFound("No se encontraron Detalles del pago.");
                }
                return Ok(pagoDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar detalles de pago");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("cargar")]
        public async Task<ActionResult> CargarConceptoDePago(PagoDetalle pagoDetalle)
        {
            try
            {
                _context.PagoDetalle.Add(pagoDetalle);
                await _context.SaveChangesAsync();
                return Ok(pagoDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de pago");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var pagoDetalle = await _context.PagoDetalle.FindAsync(id);
                if (pagoDetalle == null)
                {
                    _logger.LogWarning("No se encontró detalle de pago con ID {Id}", id);
                    return NotFound();
                }
                return Ok(pagoDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar detalle de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, PagoDetalle pagoDActualizado)
        {
            try
            {
                if (id != pagoDActualizado.PagoDetalleID)
                {
                    _logger.LogWarning("Intento de editar detalle de pago con ID no coincidente. ID recibido: {Id}, ID del detalle: {DetalleId}", id, pagoDActualizado.PagoDetalleID);
                    return BadRequest("El ID del detalle de pago no coincide.");
                }
                _context.Entry(pagoDActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(pagoDActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar detalle de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var pagoD = await _context.PagoDetalle.FindAsync(id);
                if (pagoD == null)
                {
                    _logger.LogWarning("Intento de eliminar detalle de pago no existente. ID: {Id}", id);
                    return NotFound();
                }
                _context.PagoDetalle.Remove(pagoD);
                await _context.SaveChangesAsync();
                return Ok("Detalle de Pago eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar detalle de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}