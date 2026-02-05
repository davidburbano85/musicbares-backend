using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositories
{
    public class BarRepositorioDapper : IBarRepositorio
    {
        private readonly FabricaConexion _fabricaConexion;

        public BarRepositorioDapper(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        // Crea un nuevo bar y retorna el id generado
        public async Task<int> CrearAsync(Bar bar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                INSERT INTO bar (nombre_bar, direccion, id_usuario)
                VALUES (@NombreBar, @Direccion, @IdUsuario)
                RETURNING id_bar;
            ";

            return await conexion.ExecuteScalarAsync<int>(sql, bar);
        }

        // Lista todos los bares activos del sistema
        public async Task<IEnumerable<Bar>> ListarAsync()
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT *
                FROM bar
                WHERE estado = true;
            ";

            return await conexion.QueryAsync<Bar>(sql);
        }

        // Obtiene un bar por su identificador
        public async Task<Bar?> ObtenerPorIdAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT *
                FROM bar
                WHERE id_bar = @idBar;
            ";

            return await conexion.QueryFirstOrDefaultAsync<Bar>(sql, new { idBar });
        }

        // Obtiene bares pertenecientes a un usuario específico
        public async Task<IEnumerable<Bar>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT *
                FROM bar
                WHERE id_usuario = @idUsuario
                AND estado = true;
            ";

            return await conexion.QueryAsync<Bar>(sql, new { idUsuario });
        }

        // Verifica si un bar pertenece a un usuario
        public async Task<bool> ExisteBarUsuarioAsync(int idBar, int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM bar
                WHERE id_bar = @idBar
                AND id_usuario = @idUsuario;
            ";

            var existe = await conexion.ExecuteScalarAsync<int>(sql, new { idBar, idUsuario });

            return existe > 0;
        }

        // Actualiza información de un bar existente
        public async Task<bool> ActualizarAsync(Bar bar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE bar
                SET nombre_bar = @NombreBar,
                    direccion = @Direccion
                WHERE id_bar = @IdBar;
            ";

            var filas = await conexion.ExecuteAsync(sql, bar);

            return filas > 0;
        }
    }
}
