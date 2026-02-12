using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Bar;
using Microsoft.AspNetCore.Authorization;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/bar")]
    [Authorize]
    public class BarController : ControllerBase
    {
        private readonly IBarServicio _barServicio;

        public BarController(IBarServicio barServicio)
        {
            _barServicio = barServicio;
        }

        // =====================================================
        // Ping API
        // =====================================================
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok("API funcionando correctamente");
        }

        // =====================================================
        // Debug Token
        // =====================================================
        [HttpGet("debug-token")]
        [AllowAnonymous]
        public IActionResult DebugToken()
        {
            var header = Request.Headers["Authorization"].ToString();

            string token = null;

            if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("Bearer "))
            {
                token = header.Substring("Bearer ".Length);
            }

            return Ok(new
            {
                HeaderCompleto = header,
                TokenExtraido = token,
                LargoToken = token?.Length,
                TienePuntos = token?.Count(c => c == '.')
            });
        }

        // ================================
        // Crear bar
        // ================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] BarCrearDto dto)
        {
            var resultado = await _barServicio.CrearAsync(dto);

            if (!resultado.Exitoso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ================================
        // Obtener bar por ID
        // ================================
        [HttpGet("{idBar}")]
        public async Task<IActionResult> ObtenerPorId(int idBar)
        {
            var bar = await _barServicio.ObtenerPorIdAsync(idBar);

            if (bar == null)
                return NotFound("Bar no encontrado");

            return Ok(bar);
        }

        // ================================
        // Actualizar bar
        // ================================
        [HttpPut("{idBar}")]
        public async Task<IActionResult> Actualizar(int idBar, [FromBody] BarActualizarDto dto)
        {
            if (idBar != dto.IdBar)
                return BadRequest("El id no coincide.");

            var resultado = await _barServicio.ActualizarAsync(dto);

            if (!resultado.Exitoso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ================================
        // Eliminación lógica
        // ================================
        [HttpDelete("{idBar}")]
        public async Task<IActionResult> Eliminar(int idBar)
        {
            try
            {
                var resultado = await _barServicio.EliminarAsync(idBar);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // ================================
        // Reactivar bar
        // ================================
        [HttpPatch("reactivar/{idBar}")]
        public async Task<IActionResult> Reactivar(int idBar)
        {
            try
            {
                var resultado = await _barServicio.ReactivarAsync(idBar);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // =====================================================
        // Debug Auth
        // =====================================================
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }

        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(new
            {
                autenticado = User.Identity?.IsAuthenticated,
                claims = claims
            });
        }

        // =====================================================
        // Debug Config
        // =====================================================
        [HttpGet("debug-config")]
        [AllowAnonymous]
        public IActionResult DebugConfig([FromServices] IConfiguration config)
        {
            var issuer = config["Supabase:Issuer"];
            var audience = config["Supabase:Audience"];
            var url = config["Supabase:Url"];

            return Ok(new
            {
                issuer,
                audience,
                url
            });
        }
    }
}
