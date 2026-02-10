using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories;

public interface IUsuarioRepositorio
{
    Task<int> CrearAsync(Usuario usuario);

    Task<Usuario?> ObtenerPorIdAsync(int idUsuario);

    Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico);

    Task<bool> ExisteCorreoAsync(string correoElectronico);

    Task<bool> ActualizarAsync(Usuario usuario);

    Task<IEnumerable<Usuario>> ListarAsync();

    // Cambia estado del usuario a FALSE (eliminación lógica)
    Task<bool> EliminarAsync(int idUsuario);

    // Cambia estado del usuario a TRUE (reactivar usuario)
    Task<bool> ReactivarAsync(int idUsuario);

    // Método que busca un usuario usando el auth_user_id proveniente de Supabase
    Task<Usuario?> ObtenerPorAuthIdAsync(Guid authUserId);
}
