namespace MusicBares.DTOs.VideoMesa
{
    // ======================================================
    // DTO estándar de respuesta para operaciones de VideoMesa
    // ======================================================
    public class VideoMesaRespuestaDto
    {
        public int IdVideo { get; set; }
        public int IdMesa { get; set; }
        public string LinkVideo { get; set; } = string.Empty;
        public string IdVideoYoutube { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string EstadoReproduccion { get; set; } = string.Empty;
    }
}
