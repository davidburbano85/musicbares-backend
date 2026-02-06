using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Bar;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/bar")]
    public class BarController : ControllerBase
    {
        private readonly IBarServicio _barServicio;

        public BarController(IBarServicio barServicio)
        {
            _barServicio = barServicio;
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
        // Uso administrativo o general
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
        // Importante para multi-tenant
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
        // Actualizar información de un bar
        // ================================
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] BarActualizarDto dto)
        {
            try
            {
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
        // Eliminación lógica de un bar
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

        
        [HttpPatch("reactivar/{idBar}")]
        public async Task<IActionResult> Reactivar(int idBar)
        {
            //try
            //{
            //    var resultado = await _barServicio.ReactivarAsync(idBar);

            //    return Ok(resultado);
            //}
            //catch (ArgumentException ex)
            //{
            //    return BadRequest(new { mensaje = ex.Message });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, new { mensaje = ex.Message });
            //}

            Console.WriteLine("🔥 ENTRO AL CONTROLLER");

            return Ok(new { mensaje = "🔥 CONTROLADOR NUEVO 🔥" });
        }

    }
}
