namespace MusicBares.DTOs.Usuario
{
    public class UsuarioActualizarDto
    {

        // Identificador del usuario que se va a actualizar
        public int IdUsuario { get; set; }

        // Nuevo nombre del usuario
        public string NombreCompleto { get; set; } = string.Empty;

        // Nuevo correo del usuarioa
        public string CorreoElectronico { get; set; } = string.Empty;
        //cuando quieras cambiar el correo
        public string CorreoElectronicoNuevo { get; set; } = string.Empty;

            

        // Permite activar o desactivar usuario
        public bool Estado { get; set; }
    }
}
