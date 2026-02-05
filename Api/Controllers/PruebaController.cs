using Microsoft.AspNetCore.Mvc;
using MusicBares.Infrastructure.Repositorios;

namespace MusicBares.API.Controllers
{
    [ApiController]
    [Route("api/prueba")]
    public class PruebaController : ControllerBase
    {
        private readonly PruebaConexionRepositorio _repositorio;

        public PruebaController(PruebaConexionRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet("conexion")]
        //public async Task<IActionResult> ProbarConexion()
        //{
        //    var resultado = await _repositorio.ProbarConexionAsync();
        //    return Ok(resultado);
        //}

        public async Task<string> ProbarConexionAsync()
        {
            return await Task.FromResult("API OK (sin BD)");
        }


    }
}
