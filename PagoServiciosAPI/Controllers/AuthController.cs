using PagoServiciosAPI.Data;
using PagoServiciosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ColegioPagosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PagosServiciosDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(PagosServiciosDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] Usuario usuario)
        {
            try
            {
                usuario.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return Ok(new { message = "Usuario registrado exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == login.NombreUsuario);
                if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Clave, usuario.Clave))
                {
                    _logger.LogWarning("Intento de login fallido para usuario: {NombreUsuario}", login.NombreUsuario);
                    return Unauthorized(new { message = "Credenciales inválidas." });
                }

                var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT_SECRET_KEY no está configurada.");
                    return StatusCode(500, "Error de configuración de autenticación.");
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "tu_issuer",
                    audience: "tu_audience",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(48),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Usuario {NombreUsuario} inició sesión correctamente.", usuario.NombreUsuario);
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión para usuario: {NombreUsuario}", login.NombreUsuario);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("listar")]
        public IActionResult ObtenerUsuarios()
        {
            try
            {
                var usuarios = _context.Usuarios.ToList();
                if (usuarios == null || !usuarios.Any())
                {
                    _logger.LogWarning("No se encontraron usuarios en la base de datos.");
                    return NotFound("No se encontraron usuarios.");
                }
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("editar/{id}")]
        public IActionResult EditarUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Intento de editar usuario no existente. ID: {Id}", id);
                    return NotFound("Usuario no encontrado.");
                }

                usuario.NombreUsuario = usuarioActualizado.NombreUsuario;
                usuario.role = usuarioActualizado.role;
                if (!string.IsNullOrWhiteSpace(usuarioActualizado.Clave))
                {
                    usuario.Clave = BCrypt.Net.BCrypt.HashPassword(usuarioActualizado.Clave);
                }

                _context.SaveChanges();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar usuario con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("eliminar/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Intento de eliminar usuario no existente. ID: {Id}", id);
                    return NotFound("Usuario no encontrado.");
                }

                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
                return Ok("Usuario eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}