namespace MusicBares.DTOs.Usuario;

public class UsuarioCrearDto
{
    // Nombre completo del usuario que se va a registrar
    public string NombreCompleto { get; set; } = string.Empty;

    // Correo electrónico utilizado para autenticación
    public string CorreoElectronico { get; set; } = string.Empty;

   
}
