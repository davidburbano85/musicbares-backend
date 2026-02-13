using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    public class VideoMesaServicio : IVideoMesaServicio
    {
        private readonly IVideoMesaRepositorio _repositorio;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IBarRepositorio _barRepositorio;
        private readonly IUsuarioActualServicio _usuarioActualServicio;

        public VideoMesaServicio(
            IVideoMesaRepositorio repositorio,
            IMesaRepositorio mesaRepositorio,
            IBarRepositorio barRepositorio,
            IUsuarioActualServicio usuarioActualServicio)
        {
            _repositorio = repositorio;
            _mesaRepositorio = mesaRepositorio;
            _barRepositorio = barRepositorio;
            _usuarioActualServicio = usuarioActualServicio;
        }

        // =============================================
        // Crear solicitud de video desde una mesa
        // =============================================
        public async Task<VideoMesaRespuestaDto> CrearAsync(VideoMesaCrearDto dto)
        {
            // Validar que la mesa exista
            bool mesaExiste = await _mesaRepositorio.ExisteMesaAsync(dto.IdMesa);
            if (!mesaExiste)
                throw new ArgumentException("La mesa especificada no existe.");

            var entidad = new VideoMesa
            {
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo
            };

            int idVideo = await _repositorio.CrearAsync(entidad);

            return new VideoMesaRespuestaDto
            {
                IdVideo = idVideo,
                IdMesa = dto.IdMesa,
                LinkVideo = dto.LinkVideo,
                IdVideoYoutube = null, // trigger SQL lo llena
                FechaSolicitud = DateTime.UtcNow,
                EstadoReproduccion = "Pendiente"
            };
        }

        // =============================================
        // Obtener videos de una mesa
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
        // Obtener siguiente video (round-robin) con validación
        // =============================================
        public async Task<VideoMesaRespuestaDto?> ObtenerSiguienteAsync(int idBar)
        {
            await ValidarPropietarioBarAsync(idBar);

            var video = await _repositorio.ObtenerSiguienteAsync(idBar);
            if (video == null) return null;

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
        // Obtener cola completa round-robin (con validación)
        // =============================================
        public async Task<IEnumerable<VideoMesaListadoDto>> ObtenerColaRoundRobinAsync(int idBar)
        {
            await ValidarPropietarioBarAsync(idBar);

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

        // =============================================
        // Marcar video como reproduciendo (con validación)
        // =============================================
        public async Task<bool> MarcarComoReproduciendoAsync(int idVideo)
        {
                     

            return await _repositorio.MarcarComoReproduciendoAsync(idVideo);
        }

        // =============================================
        // Eliminar video (con validación)
        // =============================================
        public async Task<bool> EliminarAsync(int idVideo)
        {
           
            return await _repositorio.EliminarAsync(idVideo);
        }

        // =============================================
        // Validación de propietario del bar
        // =============================================
        private async Task ValidarPropietarioBarAsync(int idBar)
        {
            int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
            var baresUsuario = await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario);

            if (!baresUsuario.Any(b => b.IdBar == idBar))
                throw new UnauthorizedAccessException("No tiene permiso sobre este bar.");
        }

        public async Task<VideoMesaRespuestaDto?> TomarSiguienteVideoAsync(int idBar)
        {
            // 1️⃣ Obtener el siguiente video pendiente (round-robin)
            var video = await _repositorio.ObtenerSiguienteAsync(idBar);

            if (video == null)
                return null;

            // 2️⃣ Marcar automáticamente como reproduciendo
            await _repositorio.MarcarComoReproduciendoAsync(video.IdVideo);

            // 3️⃣ Devolver DTO con la info del video
            return new VideoMesaRespuestaDto
            {
                IdVideo = video.IdVideo,
                IdMesa = video.IdMesa,
                LinkVideo = video.LinkVideo,
                IdVideoYoutube = video.IdVideoYoutube,
                FechaSolicitud = video.FechaSolicitud,
                EstadoReproduccion = "Reproduciendo"
            };
        }



    }
}
