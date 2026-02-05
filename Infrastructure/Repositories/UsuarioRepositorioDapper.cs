using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Data;

namespace MusicBares.Infrastructure.Repositories;

public class UsuarioRepositorioDapper : IUsuarioRepositorio
{
    // Fábrica para crear conexiones a la BD
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepositorioDapper(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Inserta un nuevo usuario
    public async Task<int> CrearAsync(Usuario usuario)
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = @"
            INSERT INTO usuarios 
            (nombre_completo, correo_electronico, contrasena_hash, fecha_creacion, estado)
            VALUES
            (@NombreCompleto, @CorreoElectronico, @ContrasenaHash, @FechaCreacion, @Estado)
            RETURNING id_usuario;";

        return await conexion.ExecuteScalarAsync<int>(sql, usuario);
    }

    // Obtiene usuario por Id
    public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario)
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = "SELECT * FROM usuarios WHERE id_usuario = @IdUsuario";

        return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { IdUsuario = idUsuario });
    }

    // Obtiene usuario por correo
    public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = "SELECT * FROM usuarios WHERE correo_electronico = @Correo";

        return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Correo = correoElectronico });
    }

    // Verifica si existe correo
    public async Task<bool> ExisteCorreoAsync(string correoElectronico)
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = "SELECT EXISTS (SELECT 1 FROM usuarios WHERE correo_electronico = @Correo)";

        return await conexion.ExecuteScalarAsync<bool>(sql, new { Correo = correoElectronico });
    }

    // Actualiza usuario
    public async Task<bool> ActualizarAsync(Usuario usuario)
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = @"
            UPDATE usuarios
            SET nombre_completo = @NombreCompleto,
                estado = @Estado
            WHERE id_usuario = @IdUsuario";

        int filas = await conexion.ExecuteAsync(sql, usuario);

        return filas > 0;
    }

    // Lista usuarios activos
    public async Task<IEnumerable<Usuario>> ListarAsync()
    {
        using var conexion = _connectionFactory.CrearConexion();

        string sql = "SELECT * FROM usuarios WHERE estado = true";

        return await conexion.QueryAsync<Usuario>(sql);
    }
}
