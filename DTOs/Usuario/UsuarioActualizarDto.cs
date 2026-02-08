namespace MusicBares.DTOs.Usuario
{
    public class UsuarioActualizarDto
    {

        // Identificador del usuario que se va a actualizar
        public int IdUsuario { get; set; }

        // Nuevo nombre del usuario
        public string NombreCompleto { get; set; } = string.Empty;

        // Nuevo correo del usuario
        public string CorreoElectronico { get; set; } = string.Empty;

        // Nueva contraseña en texto plano (se hashea en el servicio)
        public string? Contrasena { get; set; }

        // Permite activar o desactivar usuario
        public bool Estado { get; set; }
    }
}
