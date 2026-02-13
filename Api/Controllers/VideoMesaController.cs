using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoMesaController : ControllerBase
    {
        private readonly IVideoMesaServicio _servicio;
        private readonly IMesaRepositorio _mesaRepositorio;

        public VideoMesaController(IVideoMesaServicio servicio,
                                   IMesaRepositorio mesaRepositorio)
        {
            _servicio = servicio;
            _mesaRepositorio = mesaRepositorio;
        }

        // =========================================================
        // 1️⃣ Cliente: Registrar múltiples videos desde su mesa
        // POST: api/VideoMesa/registrar-multiples
        // =========================================================
        // =========================================================
        // 1️⃣ Cliente: Registrar múltiples videos desde su mesa usando CodigoMesa
        // POST: api/VideoMesa/registrar-multiples
        // =========================================================
        [HttpPost("registrar-multiples")] // Define la ruta del endpoint POST
        [AllowAnonymous] // Permite acceso sin autenticación (clientes del bar)
        public async Task<IActionResult> RegistrarMultiplesVideos([FromBody] VideoMesaMultipleDto request) // Recibe el body del cliente
        {
            if (request == null) // Valida que el body exista
                return BadRequest("Body requerido."); // Retorna error 400 si no hay body

            if (string.IsNullOrWhiteSpace(request.CodigoMesa)) // Valida que el código de mesa exista
                return BadRequest("CodigoMesa requerido."); // Error si no se envía el código

            if (request.Links == null || !request.Links.Any()) // Verifica que haya links enviados
                return BadRequest("Debe enviar links."); // Error si la lista está vacía

            // =========================================================
            // Resolver la mesa internamente usando CodigoMesa
            // =========================================================
            var mesa = await _mesaRepositorio.ObtenerPorCodigoQRAsync(request.CodigoMesa); // Busca la mesa por su código QR

            if (mesa == null) // Si no existe mesa con ese código
                return NotFound("Mesa no encontrada."); // Retorna 404 para evitar filtrado de información

            // =========================================================
            // Crear los videos asociados exclusivamente a esa mesa
            // =========================================================
            var resultados = new List<VideoMesaRespuestaDto>(); // Lista para acumular resultados creados

            foreach (var link in request.Links) // Itera cada link enviado por el cliente
            {
                if (!EsUrlYoutubeValida(link)) // Valida que el link sea de YouTube
                    return BadRequest($"Link inválido: {link}"); // Error si alguno no es válido

                var dto = new VideoMesaCrearDto // Construye DTO de creación interno
                {
                    IdMesa = mesa.IdMesa, // Usa el IdMesa real obtenido del backend (no del cliente)
                    LinkVideo = link // Asigna el link validado
                };

                var creado = await _servicio.CrearAsync(dto); // Crea el video usando la lógica del servicio
                resultados.Add(creado); // Agrega el resultado a la lista final
            }

            return Created("", resultados); // Retorna 201 con todos los videos creados
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
