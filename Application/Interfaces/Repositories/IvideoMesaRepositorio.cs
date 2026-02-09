using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories
{
    public interface IVideoMesaRepositorio
    {
        Task<int> CrearAsync(VideoMesa video);

        Task<IEnumerable<VideoMesa>> ObtenerPorMesaAsync(int idMesa);

        Task<VideoMesa?> ObtenerSiguienteAsync(int IdBar);

        Task<bool> EliminarAsync(int idVideo);
        Task<bool> MarcarComoReproduciendoAsync(int idVideo);
        // ======================================================
        // NUEVO: cola completa round-robin por bar
        // ======================================================
        Task<IEnumerable<VideoMesa>> ObtenerColaRoundRobinAsync(int idBar);
       
        Task<bool> CambiarEstadoAsync(int idVideo, string nuevoEstado);

    }
}
