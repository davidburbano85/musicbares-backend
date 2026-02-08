namespace MusicBares.Entidades
{
    public class Usuario
    {
        // ==========================================
        // Clave primaria autoincremental
        // ==========================================
        public int IdUsuario { get; set; }

        // ==========================================
        // Nombre completo del usuario
        // ==========================================
        public string NombreCompleto { get; set; } = string.Empty;

        // ==========================================
        // Correo único del usuario
        // ==========================================
        public string CorreoElectronico { get; set; } = string.Empty;

        // ==========================================
        // Contraseña en formato hash (bcrypt)
        // ==========================================
        public string ContrasenaHash { get; set; } = string.Empty;

        // ==========================================
        // Fecha de creación del registro
        // ==========================================
        public DateTime FechaCreacion { get; set; }

        // ==========================================
        // Estado lógico del usuario
        // TRUE = Activo
        // FALSE = Eliminado lógico
        // ==========================================
        public bool Estado { get; set; }
    }
}
