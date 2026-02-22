using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Usuario;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IBarServicio _barServicio;

        public UsuarioController(IUsuarioServicio usuarioServicio, IBarServicio barServicio)
        {
            _usuarioServicio = usuarioServicio;
            _barServicio = barServicio;
        }

        // 🔹 Listar usuarios activos (uso administrativo / pruebas)
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var usuarios = await _usuarioServicio.ListarAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al listar usuarios: {ex.Message}");
            }
        }

        // 🔹 Obtener usuario por ID
        [HttpGet("{idUsuario:int}")]
        public async Task<IActionResult> ObtenerPorId(int idUsuario)
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }

        // 🔹 Actualizar usuario
        [HttpPut("{correoElectronico}")]
        public async Task<IActionResult> Actualizar(string correoElectronico, [FromBody] UsuarioActualizarDto dto)
        {
            try
            {
                if (!string.Equals(correoElectronico, dto.CorreoElectronico, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("El correo electrónico no coincide.");

                var resultado = await _usuarioServicio.ActualizarAsync(dto);

                if (!resultado.Exitoso)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar el usuario: {ex.Message}");
            }
        }

        // 🔥 Eliminación lógica
        [HttpDelete("{idUsuario:int}")]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            var resultado = await _usuarioServicio.EliminarAsync(idUsuario);
            return Ok(resultado);
        }

        // 🔥 Reactivar usuario
        [HttpPatch("reactivar/{idUsuario}")]
        public async Task<IActionResult> Reactivar(int idUsuario)
        {
            var resultado = await _usuarioServicio.ReactivarAsync(idUsuario);
            return Ok(resultado);
        }

        // GET api/usuario/correo/{correoElectronico}
        [HttpGet("correo/{correoElectronico}")]
        public async Task<IActionResult> ObtenerPorCorreo(string correoElectronico)
        {
            var usuario = await _usuarioServicio.ObtenerPorCorreoAsync(correoElectronico);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }

        [HttpGet("{idUsuario}/mi-bar")]
        public async Task<IActionResult> ObtenerMiBar(int idUsuario)
        {
            try
            {
                // 1️⃣ Validar que el usuario exista
                var usuario = await _usuarioServicio.ObtenerPorIdAsync(idUsuario);
                if (usuario == null)
                    return NotFound(new { mensaje = "Usuario no encontrado" });

                // 2️⃣ Obtener el bar real desde BarServicio
                var bar = await _barServicio.ObtenerPrimerBarInclusoInactivoAsync(idUsuario);

                if (bar == null)
                    return NotFound(new { mensaje = "El usuario no tiene un bar asociado" });

                // 3️⃣ Devolver el IdBar real
                return Ok(new
                {
                    IdBar = bar.IdBar
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

    }
}
