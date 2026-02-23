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

        public async Task<VideoMesa?> ObtenerSiguienteAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                   WITH videos_ordenados AS (
                        SELECT
                            vm.id_video            AS IdVideo,
                            vm.id_mesa             AS IdMesa,
                            vm.link_video          AS LinkVideo,
                            vm.id_video_youtube    AS IdVideoYoutube,
                            vm.fecha_solicitud     AS FechaSolicitud,
                            vm.estado_reproduccion AS EstadoReproduccion,

                            ROW_NUMBER() OVER (
                                PARTITION BY vm.id_mesa
                                ORDER BY vm.fecha_solicitud
                            ) AS turno_mesa

                        FROM videos_mesa vm
                        INNER JOIN mesa m ON m.id_mesa = vm.id_mesa
                        WHERE
                            vm.estado_reproduccion = 'Pendiente'
                            AND m.id_bar = @idBar
                    )

                    SELECT *
                    FROM videos_ordenados
                    ORDER BY
                        turno_mesa,   -- 1º video de cada mesa
                        IdMesa;      -- orden estable
                                    ";

            return await conexion.QueryFirstOrDefaultAsync<VideoMesa>(
                sql,
                new { idBar }
            );
        }


        public async Task<bool> MarcarComoReproduciendoAsync(int idVideo)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                    UPDATE videos_mesa
                    SET estado_reproduccion = 'Reproduciendo'
                    WHERE id_video = @idVideo;
                ";

            int filas = await conexion.ExecuteAsync(
                sql,
                new { idVideo }
            );

            return filas > 0;
        }

        public async Task<IEnumerable<VideoMesa>> ObtenerColaRoundRobinAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                   WITH videos_ordenados AS (
                    SELECT
                        vm.id_video            AS IdVideo,
                        vm.id_mesa             AS IdMesa,
                        vm.link_video          AS LinkVideo,
                        vm.id_video_youtube    AS IdVideoYoutube,
                        vm.fecha_solicitud     AS FechaSolicitud,
                        vm.estado_reproduccion AS EstadoReproduccion,
                        ROW_NUMBER() OVER (
                            PARTITION BY vm.id_mesa
                            ORDER BY vm.fecha_solicitud
                        ) AS turno_mesa
                            FROM videos_mesa vm
                            INNER JOIN mesa m ON m.id_mesa = vm.id_mesa
                            WHERE vm.estado_reproduccion = 'Pendiente'
                              AND m.id_bar = @idBar
                                )
                                SELECT 
                                    IdVideo, IdMesa, LinkVideo, IdVideoYoutube, FechaSolicitud, EstadoReproduccion
                                FROM videos_ordenados
                                ORDER BY turno_mesa, IdMesa;
                ";

            return await conexion.QueryAsync<VideoMesa>(
                sql,
                new { idBar }
            );
        }


        public async Task<VideoMesa?> TomarSiguienteVideoAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            // 1️⃣ Obtener última mesa que reprodujo
            string sqlUltimaMesa = @"
                SELECT id_ultima_mesa_reproduciendo
                FROM bar
                WHERE id_bar = @idBar;
            ";

            int? idUltimaMesa = await conexion.ExecuteScalarAsync<int?>(sqlUltimaMesa, new { idBar });

            // 2️⃣ Obtener siguiente mesa con video pendiente
            string sqlSiguienteMesa = @"
                SELECT vm.id_mesa
                FROM videos_mesa vm
                INNER JOIN mesa m ON m.id_mesa = vm.id_mesa
                WHERE m.id_bar = @idBar
                  AND vm.estado_reproduccion = 'Pendiente'
                  AND (@idUltimaMesa IS NULL OR vm.id_mesa > @idUltimaMesa)
                ORDER BY vm.id_mesa ASC
                LIMIT 1;
            ";

            int? idSiguienteMesa = await conexion.ExecuteScalarAsync<int?>(sqlSiguienteMesa, new { idBar, idUltimaMesa });

            // Si no hay mesa > idUltimaMesa, tomar la primera mesa con video pendiente
            if (idSiguienteMesa == null)
            {
                string sqlPrimerMesa = @"
                    SELECT vm.id_mesa
                    FROM videos_mesa vm
                    INNER JOIN mesa m ON m.id_mesa = vm.id_mesa
                    WHERE m.id_bar = @idBar
                      AND vm.estado_reproduccion = 'Pendiente'
                    ORDER BY vm.id_mesa ASC
                    LIMIT 1;
                ";
                idSiguienteMesa = await conexion.ExecuteScalarAsync<int?>(sqlPrimerMesa, new { idBar });
            }

            if (idSiguienteMesa == null)
                return null; // No hay videos pendientes

            // 3️⃣ Tomar el primer video pendiente de esa mesa
            string sqlVideo = @"
                SELECT *
                FROM videos_mesa
                WHERE id_mesa = @idMesa
                  AND estado_reproduccion = 'Pendiente'
                ORDER BY fecha_solicitud ASC
                LIMIT 1;
            ";

            var video = await conexion.QueryFirstOrDefaultAsync<VideoMesa>(sqlVideo, new { idMesa = idSiguienteMesa });

            if (video == null)
                return null;

            // 4️⃣ Actualizar id_ultima_mesa_reproduciendo en bar
            string sqlActualizar = @"
                    UPDATE bar
                    SET id_ultima_mesa_reproduciendo = @idMesa
                    WHERE id_bar = @idBar;
                ";
            await conexion.ExecuteAsync(sqlActualizar, new { idMesa = idSiguienteMesa, idBar });

            return video;
        }

    }
}
