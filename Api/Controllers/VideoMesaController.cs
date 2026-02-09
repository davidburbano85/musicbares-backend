using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/video-mesa")]
    public class VideoMesaController : ControllerBase
    {
        private readonly IVideoMesaServicio _videoMesaServicio;

        public VideoMesaController(IVideoMesaServicio videoMesaServicio)
        {
            _videoMesaServicio = videoMesaServicio;
        }

        // ======================================================
        // CREAR VIDEO DESDE UNA MESA
        // ======================================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VideoMesaCrearDto dto)
        {
            try
            {
                var resultado = await _videoMesaServicio.CrearAsync(dto);
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

        // ======================================================
        // LISTAR VIDEOS POR MESA
        // ======================================================
        [HttpGet("mesa/{idMesa}")]
        public async Task<IActionResult> ObtenerPorMesa(int idMesa)
        {
            try
            {
                var videos = await _videoMesaServicio.ObtenerPorMesaAsync(idMesa);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // ======================================================
        // ELIMINAR VIDEO
        // ======================================================
        [HttpDelete("{idVideo}")]
        public async Task<IActionResult> Eliminar(int idVideo)
        {
            try
            {
                var resultado = await _videoMesaServicio.EliminarAsync(idVideo);
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


        [HttpGet("siguiente/{idBar}")]
        public async Task<IActionResult> ObtenerSiguiente(int idBar)
        {
            var video = await _videoMesaServicio.ObtenerSiguienteAsync(idBar);

            if (video == null)
                return NoContent(); // No hay canciones pendientes en ese bar

            return Ok(video);
        }







    }
}
