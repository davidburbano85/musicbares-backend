namespace MusicBares.DTOs.Usuario;

public class UsuarioActualizarDto
{
    // Identificador del usuario a actualizar
    public int IdUsuario { get; set; }

    // Nuevo nombre del usuario
    public string NombreCompleto { get; set; } = string.Empty;

    // Permite activar o desactivar el usuario
    public bool Estado { get; set; }
}
