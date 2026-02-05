using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace MusicBares.Infrastructure.Data;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    // Cadena de conexión obtenida desde appsettings
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    // Retorna una nueva conexión PostgreSQL
    public IDbConnection CrearConexion()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
