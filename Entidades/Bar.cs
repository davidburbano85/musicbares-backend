namespace MusicBares.Entidades
{
    /// <summary>
    /// Representa un bar dentro del sistema.
    /// 
    /// Un Bar pertenece a un Usuario (dueño del bar).
    /// 
    /// Esta clase forma parte del Dominio del negocio,
    /// por lo tanto no debe depender de tecnologías externass
    /// como base de datos, Dapper o frameworks web.
    /// </summary>
    public class Bar
    {
        /// <summary>
        /// Identificador único del bar.
        /// Este valor es generado por la base de datos.
        /// </summary>
        public int IdBar { get; set; }

        /// <summary>
        /// Nombre comercial del bar.
        /// Ejemplo: "La Cueva del Rock"
        /// </summary>
        public string NombreBar { get; set; } = string.Empty;

        /// <summary>
        /// Dirección física del bar.
        /// Permite identificar ubicación del negocio.
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del usuario dueño del bar.
        /// 
        /// Esta propiedad establece la relación jerárquica:
        /// Usuario → Bar
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Fecha en la que el bar fue registrado en el sistema.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Indica si el bar está activo o inactivo.
        /// Permite deshabilitar bares sin eliminarlos.
        /// </summary>
        public bool Estado { get; set; }
    }
}
