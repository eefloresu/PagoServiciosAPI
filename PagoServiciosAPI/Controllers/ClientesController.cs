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
    public class ClientesController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(PagosServiciosDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Esta acción requiere autenticación y el rol de Administrador
        [Authorize(Roles = "Administrador")]
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Clientes>>> Listar()
        {
            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                if (clientes == null || !clientes.Any())
                {
                    _logger.LogWarning("No se encontraron clientes.");
                    return NotFound("No se encontraron clientes.");
                }
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("cargar")]
        public async Task<ActionResult> CargarCliente(Clientes cliente)
        {
            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cliente");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Cliente,Administrador")]
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> Consultar(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null) return NotFound();
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public async Task<ActionResult> Editar(int id, Clientes clienteActualizado)
        {
            try
            {
                if (id != clienteActualizado.Id)
                {
                    _logger.LogWarning("Intento de editar cliente con ID no coincidente. ID recibido: {Id}, ID del cliente: {ClienteId}", id, clienteActualizado.Id);
                    return BadRequest("El ID del cliente no coincide.");
                }
                _context.Entry(clienteActualizado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(clienteActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Intento de eliminar cliente no existente. ID: {Id}", id);
                    return NotFound();
                }
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                return Ok("Cliente eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}