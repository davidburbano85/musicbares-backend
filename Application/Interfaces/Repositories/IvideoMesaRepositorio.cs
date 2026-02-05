using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories;

public interface IVideoMesaRepositorio
{
    // Inserta una nueva solicitud de video para una mesa
    // Retorna el Id generado del registro
    Task<int> CrearAsync(VideoMesa videoMesa);

    // Obtiene un video solicitado por su identificador único
    Task<VideoMesa?> ObtenerPorIdAsync(int idVideoMesa);

    // Obtiene todos los videos solicitados por una mesa
    // Representa la cola de reproducción de la mesa
    Task<IEnumerable<VideoMesa>> ObtenerPorMesaAsync(int idMesa);

    // Obtiene todos los videos pendientes de reproducción en una mesa
    // Permite manejar la cola activa
    Task<IEnumerable<VideoMesa>> ObtenerPendientesPorMesaAsync(int idMesa);

    // Actualiza el estado de reproducción del video
    Task<bool> ActualizarEstadoAsync(int idVideoMesa, string estadoReproduccion);

    // Elimina todos los videos de todas las mesas
    // Usado para la limpieza automática diaria (6 AM)
    Task<bool> EliminarTodosAsync();

    // Elimina todos los videos de una mesa específica
    // Puede usarse para limpiezas manuales o administración
    Task<bool> EliminarPorMesaAsync(int idMesa);

    // Lista todos los videos activos del sistema
    // Usado normalmente para administración o auditoría
    Task<IEnumerable<VideoMesa>> ListarAsync();
}
