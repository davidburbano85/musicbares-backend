using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories;

public interface IUsuarioRepositorio
{
    // Inserta un nuevo usuario en la base de datos
    // Retorna el Id generado
    Task<int> CrearAsync(Usuario usuario);

    // Obtiene un usuario por su identificador único
    Task<Usuario?> ObtenerPorIdAsync(int idUsuario);

    // Obtiene un usuario usando su correo electrónico
    // Usado principalmente para autenticación
    Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico);

    // Verifica si ya existe un correo registrado
    // Ayuda a prevenir duplicados
    Task<bool> ExisteCorreoAsync(string correoElectronico);

    // Actualiza la información de un usuario existente
    Task<bool> ActualizarAsync(Usuario usuario);

    // Lista todos los usuarios activos del sistema
    Task<IEnumerable<Usuario>> ListarAsync();
}

