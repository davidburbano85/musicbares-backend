namespace MusicBares.Entidades
{
    /// <summary>
    /// Representa un usuario dentro del sistema.
    /// 
    /// En el contexto del negocio, un Usuario es el dueño o administrador
    /// de uno o varios bares.
    /// 
    /// Esta clase pertenece al Dominio, por lo tanto:
    /// - No debe tener dependencias con base de datos.
    /// - No debe conocer Dapper ni infraestructura.
    /// - Solo representa reglas del negocio.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario.
        /// Se genera en la base de datos.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nombre completo del dueño del bar.
        /// Es obligatorio dentro del negocio.
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// Debe ser único en el sistema.
        /// Se usa para autenticación.
        /// </summary>
        public string CorreoElectronico { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario almacenada en formato HASH.
        /// Nunca debe almacenarse en texto plano.
        /// </summary>
        public string ContrasenaHash { get; set; } = string.Empty;

        /// <summary>
        /// Fecha en la que el usuario fue creado en el sistema.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Indica si el usuario está activo o inactivo.
        /// Permite deshabilitar acceso sin eliminar registros.
        /// </summary>
        public bool Estado { get; set; }
    }
}
