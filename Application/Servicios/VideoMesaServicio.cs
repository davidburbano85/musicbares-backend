using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    public class VideoMesaServicio : IVideoMesaServicio
    {
        // ======================================================
        // Repositorio de videos
        // ======================================================
        private readonly IVideoMesaRepositorio _repositorio;

        // ======================================================
        // Repositorio de mesas (nuevo)
        // Permite validar existencia y ownership
        // ======================================================
        private readonly IMesaRepositorio _mesaRepositorio;

        // ======================================================
        // Constructor con inyección de dependencias
        // ======================================================
        public VideoMesaServicio(
            IVideoMesaRepositorio repositorio,
            IMesaRepositorio mesaRepositorio)
        {
            _repositorio = repositorio;
            _mesaRepositorio = mesaRepositorio;
        }

        // =============================================
        // Crear solicitud de video desde una mesa
        // =============================================
        public async Task<VideoMesaRespuestaDto> CrearAsync(VideoMesaCrearDto dto)
        {
            // --------------------------------------------------
            // Validación 1: Verificar que la mesa exista
            // --------------------------------------------------
            bool mesaExiste = await _mesaRepositorio.ExisteMesaAsync(dto.IdMesa);

            if (!mesaExiste)
                throw new ArgumentException("La mesa especificada no existe.");

            // --------------------------------------------------
            // Validación futura (multi-tenant seguridad)
            // Aquí luego validaremos si la mesa pertenece
            // al bar del usuario autenticado
            // --------------------------------------------------

            // --------------------------------------------------
            // Construcción de entidad dominio
            // --------------------------------------------------
            var entidad = new VideoMesa
            {
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo
            };

            // --------------------------------------------------
            // Persistencia
            // --------------------------------------------------
            int idVideo = await _repositorio.CrearAsync(entidad);

            // --------------------------------------------------
            // Retorno DTO respuesta
            // --------------------------------------------------
            return new VideoMesaRespuestaDto
            {
                IdVideo = idVideo,
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo,

                // Este valor lo genera el trigger SQL
                IdVideoYoutube = null,

                // Valor informativo (no exacto)
                FechaSolicitud = DateTime.UtcNow,
                EstadoReproduccion = "Pendiente"
            };
        }

        // =============================================
        // Listar videos de una mesa
        // =============================================
        public async Task<IEnumerable<VideoMesaListadoDto>> ObtenerPorMesaAsync(int idMesa)
        {
            var videos = await _repositorio.ObtenerPorMesaAsync(idMesa);

            return videos.Select(v => new VideoMesaListadoDto
            {
                IdVideo = v.IdVideo,
                IdMesa = v.IdMesa,
                LinkVideo = v.LinkVideo,
                IdVideoYoutube = v.IdVideoYoutube,
                FechaSolicitud = v.FechaSolicitud,
                EstadoReproduccion = v.EstadoReproduccion
            });
        }

        // =============================================
        // Obtener siguiente video (round-robin por bar)
        // =============================================
        public async Task<VideoMesaRespuestaDto?> ObtenerSiguienteAsync(int idBar)
        {
            var video = await _repositorio.ObtenerSiguienteAsync(idBar);

            if (video == null)
                return null;

            return new VideoMesaRespuestaDto
            {
                IdVideo = video.IdVideo,
                IdMesa = video.IdMesa,
                LinkVideo = video.LinkVideo,
                IdVideoYoutube = video.IdVideoYoutube,
                FechaSolicitud = video.FechaSolicitud,
                EstadoReproduccion = video.EstadoReproduccion
            };
        }

        // =============================================
        // Eliminación
        // =============================================
        public async Task<bool> EliminarAsync(int idVideo)
        {
            return await _repositorio.EliminarAsync(idVideo);
        }

        // =============================================
        // Marcar como reproduciendo
        // =============================================
        public async Task<bool> MarcarComoReproduciendoAsync(int idVideo)
        {
            return await _repositorio.MarcarComoReproduciendoAsync(idVideo);
        }

        // ======================================================
        // Obtener cola completa Round Robin por bar
        // ======================================================
        public async Task<IEnumerable<VideoMesaListadoDto>> ObtenerColaRoundRobinAsync(int idBar)
        {
            var videos = await _repositorio.ObtenerColaRoundRobinAsync(idBar);

            return videos.Select(v => new VideoMesaListadoDto
            {
                IdVideo = v.IdVideo,
                IdMesa = v.IdMesa,
                LinkVideo = v.LinkVideo,
                IdVideoYoutube = v.IdVideoYoutube,
                FechaSolicitud = v.FechaSolicitud,
                EstadoReproduccion = v.EstadoReproduccion
            });
        }
    }
}
