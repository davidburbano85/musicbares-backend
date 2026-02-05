namespace MusicBares.Entidades
{
    /// <summary>
    /// Representa una solicitud de video realizada desde una mesa.
    /// 
    /// Permite registrar canciones enviadas por clientes mediante
    /// enlaces de YouTube.
    /// 
    /// Esta entidad pertenece al Dominio del negocio.
    /// </summary>
    public class VideoMesa
    {
        /// <summary>
        /// Identificador único del registro del video.
        /// Generado por la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la mesa que solicitó el video.
        /// 
        /// Relación jerárquica:
        /// Mesa → VideoMesa
        /// </summary>
        public int IdMesa { get; set; }

        /// <summary>
        /// Enlace completo del video enviado por el cliente.
        /// </summary>
        public string LinkVideo { get; set; } = string.Empty;

        /// <summary>
        /// Identificador único del video dentro de YouTube.
        /// 
        /// Este valor se extrae automáticamente desde el enlace.
        /// </summary>
        public string IdVideoYoutube { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en la que se solicitó el video.
        /// </summary>
        public DateTime FechaSolicitud { get; set; }

        /// <summary>
        /// Estado actual del video dentro del flujo de reproducción.
        /// 
        /// Ejemplo:
        /// - Pendiente
        /// - Reproduciendo
        /// - Finalizado
        /// </summary>
        public string EstadoReproduccion { get; set; } = string.Empty;
    }
}

