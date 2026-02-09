namespace MusicBares.Entidades

{
    /// <summary>
    /// Representa una mesa física dentro de un bar.
    /// 
    /// Cada mesa pertenece a un bar específico y permite
    /// que los clientes soliciten canciones desde su ubicación.
    /// 
    /// Esta entidad forma parte del Dominio del negocio,
    /// por lo tanto no depende de tecnologías externas.
    /// </summary>
    public class Mesa
    {
        /// <summary>
        /// Identificador único de la mesa.
        /// Generado por la base de datos.
        /// </summary>
        public int IdMesa { get; set; }

        /// <summary>
        /// Número visible de la mesa dentro del bar.
        /// Ejemplo: Mesa 1, Mesa 2, Mesa VIP, etc.
        /// </summary>
        public int NumeroMesa { get; set; }

        /// <summary>
        /// Identificador del bar al que pertenece la mesa.
        /// 
        /// Relación jerárquica:
        /// Bar → Mesa
        /// </summary>
        public int IdBar { get; set; }

        /// <summary>
        /// Código QR opcional que permite a los clientes acceder
        /// directamente al sistema de solicitudes desde la mesa.
        /// </summary>
        public string? CodigoQR { get; set; } 

        /// <summary>
        /// Indica si la mesa está activa dentro del sistema.
        /// Permite deshabilitar mesas temporalmente.
        /// </summary>
        public bool Estado { get; set; }
    }
}

