using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositories
{
    // ======================================================
    // Repositorio Dapper para la entidad Mesa
    // Encargado EXCLUSIVAMENTE del acceso a datos
    // ======================================================
    public class MesaRepositorioDapper : IMesaRepositorio
    {
        // ----------------------------------------------
        // Fábrica de conexiones a la base de datos
        // ----------------------------------------------
        private readonly FabricaConexion _fabricaConexion;

        // ----------------------------------------------
        // Constructor con inyección de dependencias
        // ----------------------------------------------
        public MesaRepositorioDapper(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        // ======================================================
        // CREAR MESA
        // ======================================================
        public async Task<int> CrearAsync(Mesa mesa)
        {
            // Se crea y abre la conexión
            using var conexion = _fabricaConexion.CrearConexion();

            // SQL para insertar una nueva mesa
            string sql = @"
                INSERT INTO mesa
                (numero_mesa, id_bar, codigo_qr, estado)
                VALUES
                (@NumeroMesa, @IdBar, @CodigoQr, @Estado)
                RETURNING id_mesa;
            ";

            // Ejecuta el INSERT y retorna el ID generado
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
                    codigo_qr   AS CodigoQr,
                    estado      AS Estado
                FROM mesa
                WHERE id_mesa = @idMesa;
            ";

            // QueryFirstOrDefault devuelve null si no encuentra
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
                    codigo_qr   AS CodigoQr,
                    estado      AS Estado
                FROM mesa
                WHERE id_bar = @idBar
                AND estado = true;
            ";

            return await conexion.QueryAsync<Mesa>(
                sql,
                new { idBar }
            );
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
                    codigo_qr   AS CodigoQr,
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
        // VERIFICAR SI UNA MESA PERTENECE A UN BAR
        // ======================================================
        public async Task<bool> ExisteMesaBarAsync(int idMesa, int idBar)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM mesa
                WHERE id_mesa = @idMesa
                AND id_bar = @idBar;
            ";

            int cantidad = await conexion.ExecuteScalarAsync<int>(
                sql,
                new { idMesa, idBar }
            );

            return cantidad > 0;
        }

        // ======================================================
        // VERIFICAR SI EXISTE NÚMERO DE MESA EN UN BAR
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
                    codigo_qr   = @CodigoQr,
                    estado      = @Estado
                WHERE id_mesa = @IdMesa;
            ";

            int filas = await conexion.ExecuteAsync(sql, mesa);

            return filas > 0;
        }

        // ======================================================
        // LISTAR TODAS LAS MESAS ACTIVAS
        // ======================================================
        public async Task<IEnumerable<Mesa>> ListarAsync()
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT
                    id_mesa     AS IdMesa,
                    numero_mesa AS NumeroMesa,
                    id_bar      AS IdBar,
                    codigo_qr   AS CodigoQr,
                    estado      AS Estado
                FROM mesa
                WHERE estado = true;
            ";

            return await conexion.QueryAsync<Mesa>(sql);
        }
    }
}
