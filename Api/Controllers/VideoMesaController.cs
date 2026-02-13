using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        // =========================================================
        // 1️⃣ Cliente: Registrar múltiples videos desde su mesa
        // POST: api/VideoMesa/registrar-multiples
        // =========================================================
        [HttpPost("registrar-multiples")]
        [AllowAnonymous] // Público desde la mesa
        public async Task<IActionResult> RegistrarMultiplesVideos([FromBody] VideoMesaMultipleDto request)
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

        // =========================================================
        // 2️⃣ Dueño: Obtener cola completa round-robin del bar
        // GET: api/VideoMesa/cola/{idBar}
        // =========================================================
        [HttpGet("cola/{idBar:int}")]
        [Authorize] // Solo dueño autenticado
        public async Task<IActionResult> ObtenerColaRoundRobin(int idBar)
        {
            if (idBar <= 0)
                return BadRequest("IdBar inválido.");

            var cola = await _servicio.ObtenerColaRoundRobinAsync(idBar);

            if (!cola.Any())
                return NotFound("No hay videos pendientes.");

            return Ok(cola);
        }

        // =========================================================
        // 3️⃣ Dueño: Obtener siguiente video a reproducir
        // GET: api/VideoMesa/siguiente/{idBar}
        // =========================================================
        [HttpGet("siguiente/{idBar:int}")]
        [Authorize]
        public async Task<IActionResult> ObtenerSiguiente(int idBar)
        {
            if (idBar <= 0)
                return BadRequest("IdBar inválido.");

            var video = await _servicio.TomarSiguienteVideoAsync(idBar);

            if (video == null)
                return NotFound("No existen videos pendientes.");

            return Ok(video);
        }

        // =========================================================
        // 4️⃣ Dueño: Marcar video como reproduciendo manualmente
        // PUT: api/VideoMesa/reproduciendo/{idVideo}
        // =========================================================
        [HttpPut("reproduciendo/{idVideo:int}")]
        [Authorize]
        public async Task<IActionResult> MarcarComoReproduciendo(int idVideo)
        {
            if (idVideo <= 0)
                return BadRequest("IdVideo inválido.");

            bool resultado = await _servicio.MarcarComoReproduciendoAsync(idVideo);

            if (!resultado)
                return NotFound("Video no encontrado.");

            return Ok("Estado actualizado correctamente.");
        }

        // =========================================================
        // 5️⃣ Dueño: Eliminar video de la cola
        // DELETE: api/VideoMesa/{idVideo}
        // =========================================================
        [HttpDelete("{idVideo:int}")]
        [Authorize]
        public async Task<IActionResult> Eliminar(int idVideo)
        {
            if (idVideo <= 0)
                return BadRequest("IdVideo inválido.");

            bool eliminado = await _servicio.EliminarAsync(idVideo);

            if (!eliminado)
                return NotFound("Video no encontrado.");

            return Ok("Video eliminado correctamente.");
        }

        // =========================================================
        // Validación simple de URL YouTube
        // =========================================================
        private bool EsUrlYoutubeValida(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return url.Contains("youtube.com") || url.Contains("youtu.be");
        }
    }
}
