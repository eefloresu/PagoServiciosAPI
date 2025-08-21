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
    public class PaquetesController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<PaquetesController> _logger;

        public PaquetesController(PagosServiciosDbContext context, ILogger<PaquetesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Paquete>>> Listar()
        {
            try
            {
                var paquetes = await _context.Paquetes.ToListAsync();
                if (paquetes == null || !paquetes.Any())
                {
                    _logger.LogWarning("No se encontraron paquetes.");
                    return NotFound("No se encontraron paquetes.");
                }
                return Ok(paquetes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar paquetes");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("cargar")]
        public async Task<ActionResult> CargarCliente(Paquete paquete)
        {
            try
            {
                _context.Paquetes.Add(paquete);
                await _context.SaveChangesAsync();
                return Ok(paquete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar paquete");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente, Administrador")]
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var paquete = await _context.Paquetes.FindAsync(id);
                if (paquete == null)
                {
                    _logger.LogWarning("No se encontró paquete con ID {Id}", id);
                    return NotFound();
                }
                return Ok(paquete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar paquete con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, Paquete paqueteActualizado)
        {
            try
            {
                if (id != paqueteActualizado.Id)
                {
                    _logger.LogWarning("Intento de editar paquete con ID no coincidente. ID recibido: {Id}, ID del paquete: {PaqueteId}", id, paqueteActualizado.Id);
                    return BadRequest("El ID del paquete no coincide.");
                }
                _context.Entry(paqueteActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(paqueteActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar paquete con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var paquete = await _context.Paquetes.FindAsync(id);
                if (paquete == null)
                {
                    _logger.LogWarning("Intento de eliminar paquete no existente. ID: {Id}", id);
                    return NotFound();
                }
                _context.Paquetes.Remove(paquete);
                await _context.SaveChangesAsync();
                return Ok("Paquete eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paquete con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}