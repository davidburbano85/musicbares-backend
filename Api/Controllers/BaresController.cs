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

        // GET /api/bar/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerPorUsuario(int idUsuario)
        {
            var bar = await _barServicio.ObtenerPorUsuarioAsync(idUsuario);

            if (bar == null)
                return NotFound(new { mensaje = "Bar no encontrado para este usuario" });

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

       
      
    }
}
