using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Servicios;
using MusicBares.DTOs.Usuario;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;

        public UsuarioController(UsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        // 🔹 Crear usuario
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioCrearDto dto)
        {
            var resultado = await _usuarioServicio.CrearAsync(dto);
            return Ok(resultado);
        }

        // 🔹 Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            var resultado = await _usuarioServicio.LoginAsync(dto);
            return Ok(resultado);
        }

        // 🔹 Listar usuarios activos
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _usuarioServicio.ListarAsync();
            return Ok(usuarios);
        }

        // 🔹 Obtener usuario por id
        [HttpGet("{idUsuario:int}")]
        public async Task<IActionResult> ObtenerPorId(int idUsuario)
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }

        // 🔹 Actualizar usuario
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] UsuarioActualizarDto dto)
        {
            var resultado = await _usuarioServicio.ActualizarAsync(dto);
            return Ok(resultado);
        }

        // 🔥 Eliminar usuario (lógico)
        [HttpDelete("{idUsuario:int}")]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            var resultado = await _usuarioServicio.EliminarAsync(idUsuario);
            return Ok(resultado);
        }

        // 🔥 Reactivar usuario
        [HttpPatch("reactivar/{idUsuario:int}")]
        public async Task<IActionResult> Reactivar(int idUsuario)
        {
            var resultado = await _usuarioServicio.ReactivarAsync(idUsuario);
            return Ok(resultado);
        }
    }
}
