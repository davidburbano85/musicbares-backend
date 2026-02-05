using System.Data;

namespace MusicBares.Infrastructure.Data;

public interface IDbConnectionFactory
{
    // Crea una nueva conexión hacia la base de datos
    IDbConnection CrearConexion();
}
