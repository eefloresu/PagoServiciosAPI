using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagoServiciosAPI.Data;


namespace PagoServiciosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<PagosController> _logger;

        public PagosController(PagosServiciosDbContext context, ILogger<PagosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Pago>>> Listar()
        {
            try
            {
                var pagos = await _context.Pagos.ToListAsync();
                if (pagos == null || !pagos.Any())
                {
                    _logger.LogWarning("No se encontraron pagos.");
                    return NotFound("No se encontraron pagos.");
                }
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar pagos");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpPost("cargar")]
        public async Task<ActionResult> CargarConceptoDePago(Pago pago)
        {
            try
            {
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();
                return Ok(pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar pago");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var pago = await _context.Pagos.FindAsync(id);
                if (pago == null)
                {
                    _logger.LogWarning("No se encontró pago con ID {Id}", id);
                    return NotFound();
                }
                return Ok(pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, Pago pagoActualizado)
        {
            try
            {
                if (id != pagoActualizado.PagoID)
                {
                    _logger.LogWarning("Intento de editar pago con ID no coincidente. ID recibido: {Id}, ID del pago: {PagoId}", id, pagoActualizado.PagoID);
                    return BadRequest("El ID del pago no coincide.");
                }
                _context.Entry(pagoActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(pagoActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                // Eliminar detalles asociados
                var detalles = _context.PagoDetalle.Where(d => d.PagoID == id);
                _context.PagoDetalle.RemoveRange(detalles);

                var pago = await _context.Pagos.FindAsync(id);
                if (pago == null)
                {
                    _logger.LogWarning("Intento de eliminar pago no existente. ID: {Id}", id);
                    return NotFound();
                }

                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
                return Ok("Pago eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}