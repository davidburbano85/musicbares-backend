using MusicBares.DTOs.Bar;

namespace MusicBares.Application.Interfaces.Servicios
{
    /// <summary>
    /// Define la lógica de negocio relacionada con la gestión de bares.
    /// Esta capa actúa como intermediario entre Controllers y Repositorios.
    /// </summary>
    public interface IBarServicio
    {
        /// <summary>
        /// Crea un nuevo bar en el sistema.
        /// </summary>
        Task<BarRespuestaDto> CrearAsync(BarCrearDto dto);

        /// <summary>
        /// Obtiene un bar por su identificador.
        /// </summary>
        Task<BarListadoDto?> ObtenerPorIdAsync(int idBar);

        /// <summary>
        /// Obtiene todos los bares activos del sistema.
        /// Uso administrativo.
        /// </summary>
        Task<IEnumerable<BarListadoDto>> ListarAsync();

        /// <summary>
        /// Obtiene los bares pertenecientes a un usuario.
        /// Fundamental para arquitectura multi-tenant.
        /// </summary>
        Task<IEnumerable<BarListadoDto>> ObtenerPorUsuarioAsync(int idUsuario);

        /// <summary>
        /// Actualiza la información de un bar.
        /// </summary>
        Task<BarRespuestaDto> ActualizarAsync(BarActualizarDto dto);

        /// <summary>
        /// Elimina lógicamente un bar (cambio de estado).
        /// No borra registros físicamente.
        /// </summary>
        Task<BarRespuestaDto> EliminarAsync(int idBar);

        Task<BarRespuestaDto> ReactivarAsync(int idBar);


    }
}
