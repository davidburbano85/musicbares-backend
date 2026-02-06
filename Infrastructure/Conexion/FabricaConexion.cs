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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            Console.WriteLine("🔥 CONNECTION STRING:");
            Console.WriteLine(connectionString);

            return new NpgsqlConnection(connectionString);
        }


    }
}
