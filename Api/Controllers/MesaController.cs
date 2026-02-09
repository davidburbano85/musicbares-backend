using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Mesa;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MesaController : ControllerBase
    {
        // ----------------------------------------------
        // Servicio de mesas (lógica de negocio)
        // ----------------------------------------------
        private readonly IMesaServicio _mesaServicio;

        // ----------------------------------------------
        // Constructor
        // ----------------------------------------------
        public MesaController(IMesaServicio mesaServicio)
        {
            _mesaServicio = mesaServicio;
        }

        // ======================================================
        // CREAR MESA
        // POST: api/mesa
        // ======================================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] MesaCrearDto dto)
        {
            var resultado = await _mesaServicio.CrearAsync(dto);
            return Ok(resultado);
        }

        // ======================================================
        // OBTENER MESA POR ID
        // GET: api/mesa/5
        // ======================================================
        [HttpGet("{idMesa:int}")]
        public async Task<IActionResult> ObtenerPorId(int idMesa)
        {
            var mesa = await _mesaServicio.ObtenerPorIdAsync(idMesa);

            if (mesa == null)
                return NotFound("La mesa no existe.");

            return Ok(mesa);
        }

        // ======================================================
        // OBTENER MESAS POR BAR
        // GET: api/mesa/bar/3
        // ======================================================
        [HttpGet("bar/{idBar:int}")]
        public async Task<IActionResult> ObtenerPorBar(int idBar)
        {
            var mesas = await _mesaServicio.ObtenerPorBarAsync(idBar);
            return Ok(mesas);
        }

        // ======================================================
        // OBTENER MESA POR CÓDIGO QR (flujo público)
        // GET: api/mesa/qr/ABC123
        // ======================================================
        [HttpGet("qr/{codigoQR}")]
        public async Task<IActionResult> ObtenerPorCodigoQR(string codigoQR)
        {
            var mesa = await _mesaServicio.ObtenerPorCodigoQRAsync(codigoQR);

            if (mesa == null)
                return NotFound("Mesa no encontrada.");

            return Ok(mesa);
        }

        // ======================================================
        // ACTUALIZAR MESA
        // PUT: api/mesa
        // ======================================================
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] MesaActualizarDto dto)
        {
            var resultado = await _mesaServicio.ActualizarAsync(dto);
            return Ok(resultado);
        }

        // ======================================================
        // LISTAR TODAS LAS MESAS ACTIVAS (ADMIN)
        // GET: api/mesa
        // ======================================================
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var mesas = await _mesaServicio.ListarAsync();
            return Ok(mesas);
        }
    }
}
