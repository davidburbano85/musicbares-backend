using MusicBares.DTOs.VideoMesa;

namespace MusicBares.Application.Interfaces.Servicios
{
    public interface IVideoMesaServicio
    {
        // =============================================
        // Una mesa solicita un video (inserta registro)
        // =============================================
        Task<VideoMesaRespuestaDto> CrearAsync(VideoMesaCrearDto dto);

        // =============================================
        // Obtiene los videos pendientes de una mesa
        // =============================================
        Task<IEnumerable<VideoMesaListadoDto>> ObtenerPorMesaAsync(int idMesa);

        // =============================================
        // Obtiene el siguiente video a reproducir
        // (rotación entre mesas del bar)
        // =============================================
        Task<VideoMesaRespuestaDto?> ObtenerSiguienteAsync(int IdBar);

        // =============================================
        // NUEVO: cola completa round-robin por bar
        // =============================================
        Task<IEnumerable<VideoMesaListadoDto>> ObtenerColaRoundRobinAsync(int idBar);

        // =============================================
        // Eliminación lógica (o cambio de estado)
        // =============================================
        Task<bool> EliminarAsync(int idVideo);

        Task<bool> MarcarComoReproduciendoAsync(int idVideo);


    }
}
