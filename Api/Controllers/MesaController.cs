using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Mesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/mesa")]
    [Authorize] // 🔐 Todas las mesas requieren usuario autenticado
    public class MesaController : ControllerBase
    {
        private readonly IMesaServicio _mesaServicio;

        public MesaController(IMesaServicio mesaServicio)
        {
            _mesaServicio = mesaServicio;
        }

        // ==========================================
        // Crear mesa
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
        // Obtener mesa por código QR o código interno
        // Flujo público para clientes
        // ==========================================
        [AllowAnonymous]
        [HttpGet("qr/{*codigoQR}")] // <--- Nota el '*' para capturar toda la cadena, incluso con '/'
        public async Task<IActionResult> ObtenerPorCodigoQR(string codigoQR)
        {
            try
            {
                // Decodificamos el valor por seguridad (si venía URL encoded)
                var codigo = Uri.UnescapeDataString(codigoQR);

                var mesa = await _mesaServicio.ObtenerPorCodigoQRAsync(codigo);

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


        [HttpGet("mis-mesas")]
        public async Task<IActionResult> ObtenerMisMesas()
        {
            var mesas = await _mesaServicio.ObtenerPorBarAsync(0); // Ignora el parámetro
            return Ok(mesas);
        }

    }
}
