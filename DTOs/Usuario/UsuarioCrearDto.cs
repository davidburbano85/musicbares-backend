namespace MusicBares.DTOs.Usuario;

public class UsuarioCrearDto
{
    // Nombre completo del usuario que se va a registrar
    public string NombreCompleto { get; set; } = string.Empty;

    // Correo electrónico utilizado para autenticación
    public string CorreoElectronico { get; set; } = string.Empty;

    // Contraseña en texto plano enviada desde el cliente
    // Esta contraseña será transformada a HASH dentro del servicio
    public string Contrasena { get; set; } = string.Empty;
}
