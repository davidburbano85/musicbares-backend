using Dapper;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositorios
{
    public class PruebaConexionRepositorio
    {
        private readonly FabricaConexion _fabricaConexion;

        public PruebaConexionRepositorio(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        public async Task<string> ProbarConexionAsync()
        {
            using var conexion = _fabricaConexion.CrearConexion();

            // Consulta simple al servidor PostgreSQL
            var resultado = await conexion.QueryFirstAsync<DateTime>(
                "SELECT NOW();"
            );

            return $"Conectado correctamente. Hora servidor: {resultado}";
        }
    }
}
