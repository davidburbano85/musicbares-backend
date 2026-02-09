using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    public class VideoMesaServicio : IVideoMesaServicio
    {
        private readonly IVideoMesaRepositorio _repositorio;

        public VideoMesaServicio(IVideoMesaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // =============================================
        // Crear solicitud de video desde una mesa
        // =============================================
        public async Task<VideoMesaRespuestaDto> CrearAsync(VideoMesaCrearDto dto)
        {
            var entidad = new VideoMesa
            {
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo
            };

            int idVideo = await _repositorio.CrearAsync(entidad);

            // No usamos ObtenerPorIdAsync porque NO está en la interface
            return new VideoMesaRespuestaDto
            {
                IdVideo = idVideo,
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo,
                IdVideoYoutube = null,           // lo completa SQL luego
                FechaSolicitud = DateTime.UtcNow, // valor informativo
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
                LinkVideo = v.LinkVideo,
                IdVideoYoutube = v.IdVideoYoutube,
                FechaSolicitud = v.FechaSolicitud,
                EstadoReproduccion = v.EstadoReproduccion
            });
        }

        // =============================================
        // Obtener siguiente video (round-robin por bar)
        // =============================================
        public async Task<VideoMesaRespuestaDto?> ObtenerSiguienteAsync(int IdBar)
        {
            var video = await _repositorio.ObtenerSiguienteAsync(IdBar);

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
    }
}
