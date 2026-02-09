namespace MusicBares.DTOs.VideoMesa
{
    public class VideoMesaCrearDto
    {
        public int IdMesa { get; set; }

        /// <summary>
        /// Link completo del video (YouTube u otro proveedor soportado)
        /// </summary>
        public string LinkVideo { get; set; } = string.Empty;
    }
}
