using Microsoft.AspNetCore.Mvc;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.Application.Servicios;
using MusicBares.DTOs.Usuario;
using MusicBares.Entidades;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public UsuarioController(IUsuarioServicio usuarioServicio)
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
        //{
        //    //var usuarios = await _usuarioServicio.ListarAsync();
        //    return Ok(usuarios);
        //}
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



        // 🔹 Obtener usuario por id
        [HttpGet("{idUsuario:int}")]
        public async Task<IActionResult> ObtenerPorId(int idUsuario)
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }


       //Actualizar con Correo electronico

        [HttpPut("{correoElectronico}")]
        public async Task<IActionResult> Actualizar(string correoElectronico,[FromBody] UsuarioActualizarDto dto)
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



        // 🔥 Eliminar usuario (lógico)
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
    }
}
