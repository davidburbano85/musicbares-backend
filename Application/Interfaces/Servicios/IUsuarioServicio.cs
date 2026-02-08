using MusicBares.DTOs.Usuario;

namespace MusicBares.Application.Interfaces.Servicios
{
    public interface IUsuarioServicio
    {
        // Crear usuario
        Task<UsuarioRespuestaDto> CrearAsync(UsuarioCrearDto dto);

        // Login
        Task<UsuarioRespuestaDto> LoginAsync(UsuarioLoginDto dto);

        // Obtener usuario por id
        Task<UsuarioListadoDto?> ObtenerPorIdAsync(int idUsuario);

        // Actualizar usuario
        Task<UsuarioRespuestaDto> ActualizarAsync(UsuarioActualizarDto dto);

        // Listar usuarios
        Task<IEnumerable<UsuarioListadoDto>> ListarAsync();

        
        // Eliminar usuario (soft delete)
        Task<UsuarioRespuestaDto> EliminarAsync(int idUsuario);

        // Reactivar usuario
        Task<UsuarioRespuestaDto> ReactivarAsync(int idUsuario);
    }
}
