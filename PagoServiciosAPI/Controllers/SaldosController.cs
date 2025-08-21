using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagoServiciosAPI.Data;


namespace PagoServiciosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaldosController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<SaldosController> _logger;

        public SaldosController(PagosServiciosDbContext context, ILogger<SaldosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Saldo>>> Listar()
        {
            try
            {
                var saldo = await _context.Saldos.ToListAsync();
                if (saldo == null || !saldo.Any())
                {
                    _logger.LogWarning("No se encontraron saldos.");
                    return NotFound("No se encontraron saldos.");
                }
                return Ok(saldo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar saldos");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost("cargar")]
        public async Task<ActionResult> CargarConceptoDePago(Saldo saldo)
        {
            try
            {
                _context.Saldos.Add(saldo);
                await _context.SaveChangesAsync();
                return Ok(saldo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar saldo");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var saldo = await _context.Saldos.FindAsync(id);
                if (saldo == null)
                {
                    _logger.LogWarning("No se encontró saldo con ID {Id}", id);
                    return NotFound();
                }
                return Ok(saldo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar saldo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, Saldo saldoActualizado)
        {
            try
            {
                if (id != saldoActualizado.SaldoID)
                {
                    _logger.LogWarning("Intento de editar saldo con ID no coincidente. ID recibido: {Id}, ID del saldo: {SaldoId}", id, saldoActualizado.SaldoID);
                    return BadRequest("El ID del saldo no coincide.");
                }
                _context.Entry(saldoActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(saldoActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar saldo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var saldo = await _context.Saldos.FindAsync(id);
                if (saldo == null)
                {
                    _logger.LogWarning("Intento de eliminar saldo no existente. ID: {Id}", id);
                    return NotFound();
                }
                _context.Saldos.Remove(saldo);
                await _context.SaveChangesAsync();
                return Ok("Saldo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar saldo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}