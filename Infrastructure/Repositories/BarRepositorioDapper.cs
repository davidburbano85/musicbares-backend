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

        public async Task<int> CrearAsync(Bar bar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                INSERT INTO bares (nombre, direccion, id_usuario)
                VALUES (@Nombre, @Direccion, @IdUsuario)
                RETURNING id;
            ";

            return await conexion.ExecuteScalarAsync<int>(sql, bar);
        }

        public async Task<IEnumerable<Bar>> ListarAsync()
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = "SELECT * FROM bares";

            return await conexion.QueryAsync<Bar>(sql);
        }

        public async Task<Bar?> ObtenerPorIdAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = "SELECT * FROM bares WHERE id = @idBar";

            return await conexion.QueryFirstOrDefaultAsync<Bar>(sql, new { idBar });
        }

        public async Task<IEnumerable<Bar>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = "SELECT * FROM bares WHERE id_usuario = @idUsuario";

            return await conexion.QueryAsync<Bar>(sql, new { idUsuario });
        }

        public async Task<bool> ExisteBarUsuarioAsync(int idBar, int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM bares
                WHERE id = @idBar AND id_usuario = @idUsuario
            ";

            var existe = await conexion.ExecuteScalarAsync<int>(sql, new { idBar, idUsuario });

            return existe > 0;
        }

        public async Task<bool> ActualizarAsync(Bar bar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE bares
                SET nombre = @Nombre,
                    direccion = @Direccion
                WHERE id = @Id
            ";

            var filas = await conexion.ExecuteAsync(sql, bar);

            return filas > 0;
        }
    }
}
