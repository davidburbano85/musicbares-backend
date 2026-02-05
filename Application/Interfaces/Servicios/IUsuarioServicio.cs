using MusicBares.DTOs.Usuario;

namespace MusicBares.Application.Interfaces.Servicios;

public interface IUsuarioServicio
{
    // Registra un nuevo usuario en el sistema
    // Aplica validaciones y seguridad de contraseña
    Task<UsuarioRespuestaDto> RegistrarAsync(UsuarioCrearDto dto);

    // Permite autenticación de usuario
    // Valida credenciales y retorna datos del usuario
    Task<UsuarioRespuestaDto?> LoginAsync(UsuarioLoginDto dto);

    // Obtiene información detallada de un usuario
    Task<UsuarioRespuestaDto?> ObtenerPorIdAsync(int idUsuario);

    // Permite actualizar información del usuario
    Task<bool> ActualizarAsync(UsuarioActualizarDto dto);

    // Lista todos los usuarios activos del sistema
    Task<IEnumerable<UsuarioListadoDto>> ListarAsync();
}
