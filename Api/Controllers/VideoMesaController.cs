using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;

namespace MusicBares.API.Controllers
{
    // ============================================================
    // Controller encargado de exponer endpoints REST relacionados
    // a solicitudes de videos realizadas desde mesas.
    // ============================================================
    [ApiController] // Indica que es un controlador Web API
    [Route("api/[controller]")] // Ruta base -> api/VideoMesa
    public class VideoMesaController : ControllerBase
    {
        // Servicio de capa Application
        private readonly IVideoMesaServicio _servicio;

        // ============================================================
        // Constructor con Inyección de Dependencias
        // ============================================================
        public VideoMesaController(IVideoMesaServicio servicio)
        {
            _servicio = servicio;
        }

        // ============================================================
        // 1️⃣ Registrar múltiples videos para una mesa
        // POST: api/VideoMesa/registrar-multiples
        // ============================================================
        [HttpPost("registrar-multiples")]
        public async Task<IActionResult> RegistrarMultiplesVideos(
      [FromBody] VideoMesaMultipleDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Body requerido.");

                if (request.IdMesa <= 0)
                    return BadRequest("IdMesa inválido.");

                if (!request.Links.Any())
                    return BadRequest("Debe enviar links.");

                var resultados = new List<VideoMesaRespuestaDto>();

                foreach (var link in request.Links)
                {
                    if (!EsUrlYoutubeValida(link))
                        return BadRequest($"Link inválido: {link}");

                    var dto = new VideoMesaCrearDto
                    {
                        IdMesa = request.IdMesa,
                        LinkVideo = link
                    };

                    var creado = await _servicio.CrearAsync(dto);

                    resultados.Add(creado);
                }

                return Created("", resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // 2️⃣ Obtener videos solicitados por una mesa
        // GET: api/VideoMesa/mesa/{idMesa}
        // ============================================================
        [HttpGet("mesa/{idMesa:int}")]
        public async Task<IActionResult> ObtenerPorMesa(int idMesa)
        {
            try
            {
                if (idMesa <= 0)
                    return BadRequest("IdMesa inválido.");

                var videos = await _servicio.ObtenerPorMesaAsync(idMesa);

                if (!videos.Any())
                    return NotFound("No existen videos para esta mesa.");

                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // 3️⃣ Obtener cola Round Robin del bar
        // GET: api/VideoMesa/cola/{idBar}
        // ============================================================
        [HttpGet("cola/{idBar:int}")]
        public async Task<IActionResult> ObtenerColaRoundRobin(int idBar)
        {
            try
            {
                if (idBar <= 0)
                    return BadRequest("IdBar inválido.");

                var cola = await _servicio.ObtenerColaRoundRobinAsync(idBar);

                if (!cola.Any())
                    return NotFound("No hay videos pendientes.");

                return Ok(cola);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // 4️⃣ Obtener siguiente video del bar
        // GET: api/VideoMesa/siguiente/{idBar}
        // ============================================================
        [HttpGet("siguiente/{idBar:int}")]
        public async Task<IActionResult> ObtenerSiguiente(int idBar)
        {
            try
            {
                if (idBar <= 0)
                    return BadRequest("IdBar inválido.");

                var video = await _servicio.ObtenerSiguienteAsync(idBar);

                if (video == null)
                    return NotFound("No existen videos pendientes.");

                return Ok(video);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // 5️⃣ Marcar video como reproduciendo
        // PUT: api/VideoMesa/reproduciendo/{idVideo}
        // ============================================================
        [HttpPut("reproduciendo/{idVideo:int}")]
        public async Task<IActionResult> MarcarComoReproduciendo(int idVideo)
        {
            try
            {
                if (idVideo <= 0)
                    return BadRequest("IdVideo inválido.");

                bool resultado = await _servicio
                    .MarcarComoReproduciendoAsync(idVideo);

                if (!resultado)
                    return NotFound("Video no encontrado.");

                return Ok("Estado actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // 6️⃣ Eliminar solicitud de video
        // DELETE: api/VideoMesa/{idVideo}
        // ============================================================
        [HttpDelete("{idVideo:int}")]
        public async Task<IActionResult> Eliminar(int idVideo)
        {
            try
            {
                if (idVideo <= 0)
                    return BadRequest("IdVideo inválido.");

                bool eliminado = await _servicio.EliminarAsync(idVideo);

                if (!eliminado)
                    return NotFound("Video no encontrado.");

                return Ok("Video eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ============================================================
        // Validación simple de URL YouTube
        // ============================================================
        private bool EsUrlYoutubeValida(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return url.Contains("youtube.com") || url.Contains("youtu.be");
        }
    }
}
