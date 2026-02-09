namespace MusicBares.DTOs.VideoMesa
{
    public class VideoMesaRespuestaDto
    {
        public int IdVideo { get; set; }

        public int IdMesa { get; set; }

        public string LinkVideo { get; set; } = string.Empty;

        /// <summary>
        /// Valor generado automáticamente por SQL (trigger).
        /// El cliente NO lo envía.
        /// </summary>
        public string? IdVideoYoutube { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public string EstadoReproduccion { get; set; } = string.Empty;
    }
}
