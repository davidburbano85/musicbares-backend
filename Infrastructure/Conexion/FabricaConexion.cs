using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MusicBares.Infrastructure.Conexion
{
    public class FabricaConexion
    {
        private readonly IConfiguration _configuration;

        // Inyectamos IConfiguration para leer appsettings.json
        public FabricaConexion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Método que crea una conexión nueva a PostgreSQL
        public IDbConnection CrearConexion()
        {
            // Obtiene la cadena de conexión desde appsettings
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Retorna conexión PostgreSQL
            return new NpgsqlConnection(connectionString);
        }
    }
}
