using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositories
{
    // ======================================================
    // Repositorio Dapper para la entidad Mesa
    // Acceso EXCLUSIVO a datos
    // ======================================================
    public class MesaRepositorioDapper : IMesaRepositorio
    {
        private readonly FabricaConexion _fabricaConexion;

        public MesaRepositorioDapper(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        // ======================================================
        // CREAR MESA
        // ======================================================
        public async Task<int> CrearAsync(Mesa mesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                INSERT INTO mesa
                    (numero_mesa, id_bar, codigo_qr, estado)
                VALUES
                    (@NumeroMesa, @IdBar, @CodigoQR, @Estado)
                RETURNING id_mesa;
            ";

            return await conexion.ExecuteScalarAsync<int>(sql, mesa);
        }

        // ======================================================
        // OBTENER MESA POR ID
        // ======================================================
        public async Task<Mesa?> ObtenerPorIdAsync(int idMesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_mesa     AS IdMesa,
                    numero_mesa AS NumeroMesa,
                    id_bar      AS IdBar,
                    codigo_qr   AS CodigoQR,
                    estado      AS Estado
                FROM mesa
                WHERE id_mesa = @idMesa;
            ";

            return await conexion.QueryFirstOrDefaultAsync<Mesa>(
                sql,
                new { idMesa }
            );
        }

        // ======================================================
        // OBTENER MESAS POR BAR
        // ======================================================
        public async Task<IEnumerable<Mesa>> ObtenerPorBarAsync(int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_mesa     AS IdMesa,
                    numero_mesa AS NumeroMesa,
                    id_bar      AS IdBar,
                    codigo_qr   AS CodigoQR,
                    estado      AS Estado
                FROM mesa
                WHERE id_bar = @idBar
                  AND estado = true;
            ";

            return await conexion.QueryAsync<Mesa>(sql, new { idBar });
        }

        // ======================================================
        // OBTENER MESA POR CÓDIGO QR
        // ======================================================
        public async Task<Mesa?> ObtenerPorCodigoQRAsync(string codigoQR)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_mesa     AS IdMesa,
                    numero_mesa AS NumeroMesa,
                    id_bar      AS IdBar,
                    codigo_qr   AS CodigoQR,
                    estado      AS Estado
                FROM mesa
                WHERE codigo_qr = @codigoQR
                  AND estado = true;
            ";

            return await conexion.QueryFirstOrDefaultAsync<Mesa>(
                sql,
                new { codigoQR }
            );
        }

        // ======================================================
        // VALIDAR SI MESA PERTENECE A UN BAR
        // ======================================================
        public async Task<bool> ExisteMesaBarAsync(int idMesa, int idBar)
        {
            if (idBar <= 0) return false;
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM mesa
                WHERE id_mesa = @idMesa
                  AND id_bar  = @idBar;
            ";

            int cantidad = await conexion.ExecuteScalarAsync<int>(
                sql,
                new { idMesa, idBar }
            );

            return cantidad > 0;
        }

        // ======================================================
        // VALIDAR NÚMERO DE MESA REPETIDO EN UN BAR
        // ======================================================
        public async Task<bool> ExisteNumeroMesaAsync(int idBar, int numeroMesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM mesa
                WHERE id_bar = @idBar
                  AND numero_mesa = @numeroMesa;
            ";

            int cantidad = await conexion.ExecuteScalarAsync<int>(
                sql,
                new { idBar, numeroMesa }
            );

            return cantidad > 0;
        }

        // ======================================================
        // ACTUALIZAR MESA
        // ======================================================
        public async Task<bool> ActualizarAsync(Mesa mesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE mesa
                SET
                    numero_mesa = @NumeroMesa,
                    codigo_qr   = @CodigoQR,
                    estado      = @Estado
                WHERE id_mesa = @IdMesa;
            ";

            int filas = await conexion.ExecuteAsync(sql, mesa);
            return filas > 0;
        }

        // ======================================================
        // LISTAR MESAS ACTIVAS
        // ======================================================
       
        public async Task<bool> ExisteMesaAsync(int idMesa)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            var sql = @"
                SELECT EXISTS (
                    SELECT 1
                    FROM mesa
                    WHERE id_mesa = @IdMesa
                );
            ";

            return await conexion.ExecuteScalarAsync<bool>(sql, new { IdMesa = idMesa });
        }


    }
}
