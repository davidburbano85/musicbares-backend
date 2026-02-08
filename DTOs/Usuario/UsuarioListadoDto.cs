namespace MusicBares.DTOs.Usuario
{
    /// DTO utilizado para listar usuarios en tablas o vistas administrativas
    /// Contiene únicamente información visible y segura
    public class UsuarioListadoDto
    {
        /// Identificador único del usuario
        public int IdUsuario { get; set; }

        /// Nombre completo del usuario
        public string NombreCompleto { get; set; } = string.Empty;

        /// Correo electrónico del usuario
        public string CorreoElectronico { get; set; } = string.Empty;

        /// Fecha en la que se creó el usuario
        public DateTime FechaCreacion { get; set; }

        /// Indica si el usuario está activo o eliminado lógicamente
        /// TRUE = Activo
        /// FALSE = Eliminado / Inactivo
        public bool Estado { get; set; }
    }
}
