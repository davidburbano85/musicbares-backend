namespace MusicBares.Entidades
{
    // ======================================================
    // Entidad: VideosMesa
    // Representa una solicitud de video realizada desde una mesa
    // ======================================================
    public class VideoMesa
    {
        // ==========================================
        // Clave primaria autoincremental
        // Corresponde a: id_video
        // ==========================================
        public int IdVideo { get; set; }

        // ==========================================
        // Clave foránea hacia la mesa
        // Relación: videos_mesa → mesa
        // ==========================================
        public int IdMesa { get; set; }

        // ==========================================
        // Link completo del video de YouTube
        // Ejemplo:
        // https://www.youtube.com/watch?v=ABC123
        // ==========================================
        public string LinkVideo { get; set; } = string.Empty;

        // ==========================================
        // ID limpio del video de YouTube
        // Este valor NO lo genera C#
        // Lo genera PostgreSQL con la función:
        // extraer_id_youtube(link_video)
        // ==========================================
        public string IdVideoYoutube { get; set; } = string.Empty;

        // ==========================================
        // Fecha y hora en la que se solicitó el video
        // Normalmente la asigna la base de datos (NOW())
        // ==========================================
        public DateTime FechaSolicitud { get; set; }

        // ==========================================
        // Estado actual del video en la cola
        // Valores típicos:
        // - Pendiente
        // - Reproduciendo
        // - Finalizado
        // ==========================================
        public string? EstadoReproduccion { get; set; } 
    }
}
