using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositories
{
    // ======================================================
    // Repositorio Dapper para videos solicitados desde mesas
    // ======================================================
    public class VideoMesaRepositorioDapper : IVideoMesaRepositorio
    {
        private readonly FabricaConexion _fabricaConexion;

        public VideoMesaRepositorioDapper(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        // ======================================================
        // CREAR VIDEO (el trigger SQL hace la magia)
        // ======================================================
        public async Task<int> CrearAsync(VideoMesa videoMesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                INSERT INTO videos_mesa
                (id_mesa, link_video)
                VALUES
                (@IdMesa, @LinkVideo)
                RETURNING id_video;
            ";

            return await conexion.ExecuteScalarAsync<int>(sql, videoMesa);
        }

        // ======================================================
        // OBTENER VIDEO POR ID
        // ======================================================
        public async Task<VideoMesa?> ObtenerPorIdAsync(int idVideo)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_video            AS IdVideo,
                    id_mesa             AS IdMesa,
                    link_video          AS LinkVideo,
                    id_video_youtube    AS IdVideoYoutube,
                    fecha_solicitud     AS FechaSolicitud,
                    estado_reproduccion AS EstadoReproduccion
                FROM videos_mesa
                WHERE id_video = @idVideo;
            ";

            return await conexion.QueryFirstOrDefaultAsync<VideoMesa>(
                sql,
                new { idVideo }
            );
        }

        // ======================================================
        // LISTAR VIDEOS POR MESA
        // ======================================================
        public async Task<IEnumerable<VideoMesa>> ObtenerPorMesaAsync(int idMesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_video            AS IdVideo,
                    id_mesa             AS IdMesa,
                    link_video          AS LinkVideo,
                    id_video_youtube    AS IdVideoYoutube,
                    fecha_solicitud     AS FechaSolicitud,
                    estado_reproduccion AS EstadoReproduccion
                FROM videos_mesa
                WHERE id_mesa = @idMesa
                ORDER BY fecha_solicitud ASC;
            ";

            return await conexion.QueryAsync<VideoMesa>(
                sql,
                new { idMesa }
            );
        }

        // ======================================================
        // CAMBIAR ESTADO DE REPRODUCCIÓN
        // ======================================================
        public async Task<bool> CambiarEstadoAsync(int idVideo, string nuevoEstado)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE videos_mesa
                SET estado_reproduccion = @nuevoEstado
                WHERE id_video = @idVideo;
            ";

            int filas = await conexion.ExecuteAsync(
                sql,
                new { idVideo, nuevoEstado }
            );

            return filas > 0;
        }

        // ======================================================
        // ELIMINAR VIDEO (borrado físico)
        // ======================================================
        public async Task<bool> EliminarAsync(int idVideo)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                DELETE FROM videos_mesa
                WHERE id_video = @idVideo;
            ";

            int filas = await conexion.ExecuteAsync(
                sql,
                new { idVideo }
            );

            return filas > 0;
        }

        public async Task<VideoMesa?> ObtenerSiguienteAsync(int IdBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                WITH videos_ordenados AS (
                    SELECT
                        vm.id_video,
                        vm.id_mesa,
                        vm.link_video,
                        vm.id_video_youtube,
                        vm.fecha_solicitud,
                        vm.estado_reproduccion,

                        -- Numera los videos por mesa
                        ROW_NUMBER() OVER (
                            PARTITION BY vm.id_mesa
                            ORDER BY vm.fecha_solicitud
                        ) AS turno_mesa

                    FROM videos_mesa vm
                    WHERE vm.estado_reproduccion = 'Pendiente'
                )
                SELECT *
                FROM videos_ordenados
                ORDER BY turno_mesa, id_mesa
                LIMIT 1;
            ";

            return await conexion.QueryFirstOrDefaultAsync<VideoMesa>(sql);
        }


    }
}
