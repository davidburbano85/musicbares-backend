using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Mesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/mesa")]
    public class MesaController : ControllerBase
    {
        // ==========================================
        // Servicio de mesa (lógica de negocio)
        // ==========================================
        private readonly IMesaServicio _mesaServicio;

        // ==========================================
        // Constructor con inyección de dependencias
        // ==========================================
        public MesaController(IMesaServicio mesaServicio)
        {
            _mesaServicio = mesaServicio;
        }

        // ==========================================
        // Crear una nueva mesa
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] MesaCrearDto dto)
        {
            try
            {
                var resultado = await _mesaServicio.CrearAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al crear la mesa: {ex.Message}");
            }
        }

        // ==========================================
        // Obtener mesa por ID
        // ==========================================
        [HttpGet("{idMesa}")]
        public async Task<IActionResult> ObtenerPorId(int idMesa)
        {
            try
            {
                var mesa = await _mesaServicio.ObtenerPorIdAsync(idMesa);

                if (mesa == null)
                    return NotFound("Mesa no encontrada");

                return Ok(mesa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener la mesa: {ex.Message}");
            }
        }

        // ==========================================
        // Listar todas las mesas (administrativo)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var mesas = await _mesaServicio.ListarAsync();
                return Ok(mesas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al listar mesas: {ex.Message}");
            }
        }

        // ==========================================
        // Obtener mesas por bar
        // ==========================================
        [HttpGet("bar/{idBar}")]
        public async Task<IActionResult> ObtenerPorBar(int idBar)
        {
            try
            {
                var mesas = await _mesaServicio.ObtenerPorBarAsync(idBar);
                return Ok(mesas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener mesas del bar: {ex.Message}");
            }
        }

        // ==========================================
        // Obtener mesa por código QR
        // ==========================================
        [HttpGet("qr/{codigoQR}")]
        public async Task<IActionResult> ObtenerPorCodigoQR(string codigoQR)
        {
            try
            {
                var mesa = await _mesaServicio.ObtenerPorCodigoQRAsync(codigoQR);

                if (mesa == null)
                    return NotFound("Mesa no encontrada");

                return Ok(mesa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener mesa por QR: {ex.Message}");
            }
        }

        // ==========================================
        // Actualizar mesa
        // ==========================================
        [HttpPut("{idMesa}")]
        public async Task<IActionResult> Actualizar(int idMesa, [FromBody] MesaActualizarDto dto)
        {
            try
            {
                if (idMesa != dto.IdMesa)
                    return BadRequest("El id de la mesa no coincide.");

                var resultado = await _mesaServicio.ActualizarAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar la mesa: {ex.Message}");
            }
        }
    }
}
