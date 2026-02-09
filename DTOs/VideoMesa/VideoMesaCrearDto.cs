namespace MusicBares.DTOs.VideoMesa
{
    // ======================================================
    // DTO usado para solicitar un video desde una mesa
    // ======================================================
    public class VideoMesaCrearDto
    {
        // ==========================================
        // ID de la mesa que solicita el video
        // ==========================================
        public int IdMesa { get; set; }

        // ==========================================
        // Link completo de YouTube enviado por el usuario
        // ==========================================
        public string LinkVideo { get; set; } = string.Empty;
    }
}
