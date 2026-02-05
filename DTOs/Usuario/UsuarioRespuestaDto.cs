namespace MusicBares.DTOs.Usuario;

public class UsuarioRespuestaDto
{
    // Identificador del usuario
    public int IdUsuario { get; set; }

    // Nombre del usuario
    public string NombreCompleto { get; set; } = string.Empty;

    // Correo electrónico del usuario
    public string CorreoElectronico { get; set; } = string.Empty;

    // Fecha de creación del usuario
    public DateTime FechaCreacion { get; set; }

    // Estado del usuario
    public bool Estado { get; set; }
}
