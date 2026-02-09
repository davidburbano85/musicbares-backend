using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoMesaController : ControllerBase
    {
        private readonly IVideoMesaServicio _servicio;

        public VideoMesaController(IVideoMesaServicio servicio)
        {
            _servicio = servicio;
        }

        // =============================================
        // Una mesa solicita un video
        // POST: api/VideoMesa
        // =============================================
        [HttpPost]
        public async Task<ActionResult<VideoMesaRespuestaDto>> Crear(
            [FromBody] VideoMesaCrearDto dto)
        {
            var resultado = await _servicio.CrearAsync(dto);
            return Ok(resultado);
        }

        // =============================================
        // Obtener videos de una mesa
        // GET: api/VideoMesa/mesa/5
        // =============================================
        [HttpGet("mesa/{idMesa:int}")]
        public async Task<ActionResult<IEnumerable<VideoMesaListadoDto>>> ObtenerPorMesa(
            int idMesa)
        {
            var lista = await _servicio.ObtenerPorMesaAsync(idMesa);
            return Ok(lista);
        }

        // =============================================
        // Obtener siguiente video a reproducir (bar)
        // GET: api/VideoMesa/siguiente/3
        // =============================================
        [HttpGet("siguiente/{idBar:int}")]
        public async Task<ActionResult<VideoMesaRespuestaDto>> ObtenerSiguiente(
            int idBar)
        {
            var video = await _servicio.ObtenerSiguienteAsync(idBar);

            if (video == null)
                return NoContent();

            return Ok(video);
        }

        // =============================================
        // Eliminar video
        // DELETE: api/VideoMesa/10
        // =============================================
        [HttpDelete("{idVideo:int}")]
        public async Task<IActionResult> Eliminar(int idVideo)
        {
            bool eliminado = await _servicio.EliminarAsync(idVideo);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }

        // =============================================
        // Marcar video como reproduciendo
        // PUT: api/VideoMesa/reproduciendo/10
        // =============================================
        [HttpPut("reproduciendo/{idVideo:int}")]
        public async Task<IActionResult> MarcarComoReproduciendo(int idVideo)
        {
            bool actualizado = await _servicio.MarcarComoReproduciendoAsync(idVideo);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        // NUEVO: Cola completa round-robin por bar
        // GET: api/VideoMesa/cola/{idBar}
        // ======================================================
        [HttpGet("cola/{idBar:int}")]
        public async Task<ActionResult<IEnumerable<VideoMesaListadoDto>>> ObtenerColaRoundRobin(
            int idBar)
        {
            var cola = await _servicio.ObtenerColaRoundRobinAsync(idBar);
            return Ok(cola);
        }
    }
}
