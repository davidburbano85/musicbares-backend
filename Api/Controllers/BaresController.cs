using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Bar;
using Microsoft.AspNetCore.Authorization;

namespace MusicBares.API.Controllers
{
    // Indica que este controlador es una API
    [ApiController]

    // Ruta base del controlador
    [Route("api/bar")]

    // Obliga a que todos los endpoints requieran autenticación
   // [Authorize]
    public class BarController : ControllerBase
    {
        // Servicio que contiene la lógica de negocio de Bar
        private readonly IBarServicio _barServicio;

        // Constructor que recibe el servicio por inyección de dependencias
        public BarController(IBarServicio barServicio)
        {
            _barServicio = barServicio;
        }

        // =====================================================
        // 🔥 ENDPOINT SOLO PARA SABER SI LA API RESPONDE
        // NO REQUIERE TOKEN
        // =====================================================
        [HttpGet("ping")]
        [AllowAnonymous] // Permite entrar sin autenticación
        public IActionResult Ping()
        {
            return Ok("API funcionando correctamente");
        }

        // =====================================================
        // 🔥 ENDPOINT PARA VER SI JWT FUNCIONA
        // =====================================================
        [HttpGet("debug-token")]
        [AllowAnonymous]
        public IActionResult DebugToken()
        {
            var header = Request.Headers["Authorization"].ToString();

            return Ok(new
            {
                Header = header,
                TieneBearer = header.StartsWith("Bearer "),
                UserAutenticado = User?.Identity?.IsAuthenticated
            });
        }

        // ================================
        // Crear un nuevo bar
        // ================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] BarCrearDto dto)
        {
            try
            {
                var resultado = await _barServicio.CrearAsync(dto);

                if (!resultado.Exitoso)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al crear el bar: {ex.Message}");
            }
        }

        // ================================
        // Obtener bar por ID
        // ================================
        [HttpGet("{idBar}")]
        public async Task<IActionResult> ObtenerPorId(int idBar)
        {
            try
            {
                var bar = await _barServicio.ObtenerPorIdAsync(idBar);

                if (bar == null)
                    return NotFound("Bar no encontrado");

                return Ok(bar);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener el bar: {ex.Message}");
            }
        }

        // ================================
        // Listar todos los bares
        // ================================
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var bares = await _barServicio.ListarAsync();
                return Ok(bares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al listar bares: {ex.Message}");
            }
        }

        // ================================
        // Obtener bares por usuario
        // ================================
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerPorUsuario(int idUsuario)
        {
            try
            {
                var bares = await _barServicio.ObtenerPorUsuarioAsync(idUsuario);
                return Ok(bares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener bares del usuario: {ex.Message}");
            }
        }

        // ================================
        // Actualizar bar
        // ================================
        [HttpPut("{IdBar}")]
        public async Task<IActionResult> Actualizar(int IdBar, [FromBody] BarActualizarDto dto)
        {
            try
            {
                if (IdBar != dto.IdBar)
                    return BadRequest("El id no coincide.");

                var resultado = await _barServicio.ActualizarAsync(dto);

                if (!resultado.Exitoso)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar el bar: {ex.Message}");
            }
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
    }
}
