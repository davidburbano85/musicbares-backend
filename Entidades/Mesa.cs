namespace MusicBares.Entidades

{
    
    /// Representa una mesa física dentro de un bar.
    /// 
    /// Cada mesa pertenece a un bar específico y permite
    /// que los clientes soliciten canciones desde su ubicación.
    /// 
    /// Esta entidad forma parte del Dominio del negocio,
    /// por lo tanto no depende de tecnologías externas.
   
    public class Mesa
    {
        /// Identificador único de la mesa.
        /// Generado por la base de datos.
        public int IdMesa { get; set; }

        /// Número visible de la mesa dentro del bar.
        /// Ejemplo: Mesa 1, Mesa 2, Mesa VIP, etc.
        public int NumeroMesa { get; set; }

        /// Identificador del bar al que pertenece la mesa.
        /// 
        /// Relación jerárquica:
        /// Bar → Mesa
        public int IdBar { get; set; }

        /// Código QR opcional que permite a los clientes acceder
        /// directamente al sistema de solicitudes desde la mesa.
        public string? CodigoQR { get; set; } 

        /// Indica si la mesa está activa dentro del sistema.
        /// Permite deshabilitar mesas temporalmente.
        public bool Estado { get; set; }
    }
}

