using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagoServiciosAPI.Data;
using PagoServiciosAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace PagoServiciosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConceptoPagosController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<ConceptoPagosController> _logger;

        public ConceptoPagosController(PagosServiciosDbContext context, ILogger<ConceptoPagosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<ConceptosPago>>> Listar()
        {
            try
            {
                var conceptopago = await _context.ConceptosPago.ToListAsync();
                if (conceptopago == null || !conceptopago.Any())
                {
                    _logger.LogWarning("No se encontraron conceptos de pago.");
                    return NotFound("No se encontraron conceptos de pago.");
                }
                return Ok(conceptopago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar conceptos de pago");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("cargar")]
        public async Task<ActionResult> CargarConceptoDePago(ConceptosPago concepto)
        {
            try
            {
                _context.ConceptosPago.Add(concepto);
                await _context.SaveChangesAsync();
                return Ok(concepto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar concepto de pago");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var concepto = await _context.ConceptosPago.FindAsync(id);
                if (concepto == null)
                {
                    _logger.LogWarning("No se encontró concepto de pago con ID {Id}", id);
                    return NotFound();
                }
                return Ok(concepto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar concepto de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, ConceptosPago conceptoActualizado)
        {
            try
            {
                if (id != conceptoActualizado.Id)
                {
                    _logger.LogWarning("Intento de editar concepto de pago con ID no coincidente. ID recibido: {Id}, ID del concepto: {ConceptoId}", id, conceptoActualizado.Id);
                    return BadRequest("El ID del concepto no coincide.");
                }
                _context.Entry(conceptoActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(conceptoActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar concepto de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var conceptoPago = await _context.ConceptosPago.FindAsync(id);
                if (conceptoPago == null)
                {
                    _logger.LogWarning("Intento de eliminar concepto de pago no existente. ID: {Id}", id);
                    return NotFound();
                }
                _context.ConceptosPago.Remove(conceptoPago);
                await _context.SaveChangesAsync();
                return Ok("Concepto de pago eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar concepto de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}