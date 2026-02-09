namespace MusicBares.DTOs.VideoMesa
{
    // ======================================================
    // DTO para listar videos solicitados por mesas
    // ======================================================
    public class VideoMesaListadoDto
    {
        public int IdVideo { get; set; }

        public int IdMesa { get; set; }

        // Link original (útil para frontend)
        public string LinkVideo { get; set; } = string.Empty;

        // ID limpio de YouTube (ideal para embeds)
        public string IdVideoYoutube { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public string EstadoReproduccion { get; set; } = string.Empty;
    }
}
