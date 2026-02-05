namespace MusicBares.DTOs.Usuario;

public class UsuarioLoginDto
{
    // Correo electrónico usado para iniciar sesión
    public string CorreoElectronico { get; set; } = string.Empty;

    // Contraseña en texto plano enviada por el cliente
    public string Contrasena { get; set; } = string.Empty;
}
