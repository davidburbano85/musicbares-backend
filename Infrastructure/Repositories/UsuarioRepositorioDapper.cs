using Dapper;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Entidades;
using MusicBares.Infrastructure.Conexion;

namespace MusicBares.Infrastructure.Repositorios
{
    public class UsuarioRepositorioDapper : IUsuarioRepositorio
    {
        // Fábrica que crea la conexión a la base de datos
        private readonly FabricaConexion _fabricaConexion;


        // Inyección de dependencias
        public UsuarioRepositorioDapper(FabricaConexion fabricaConexion)
        {
            _fabricaConexion = fabricaConexion;
        }

        // ================================
        // CREAR USUARIO
        // ================================
        public async Task<int> CrearAsync(Usuario usuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                INSERT INTO usuario
                (nombre_completo, correo_electronico, contrasena_hash, estado)
                VALUES
                (@NombreCompleto, @CorreoElectronico, @ContrasenaHash, @Estado)
                RETURNING id_usuario;
            ";

            // Ejecuta el INSERT y retorna el id generado
            return await conexion.ExecuteScalarAsync<int>(sql, usuario);
        }

        // ================================
        // OBTENER POR ID
        // ================================
        public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT 
                    id_usuario AS IdUsuario,
                    nombre_completo AS NombreCompleto,
                    correo_electronico AS CorreoElectronico,
                    contrasena_hash AS ContrasenaHash,
                    fecha_creacion AS FechaCreacion,
                    estado AS Estado
                FROM usuario
                WHERE id_usuario = @idUsuario;
            ";

            return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { idUsuario });
        }

        // ================================
        // OBTENER POR CORREO
        // ================================
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT 
                    id_usuario AS IdUsuario,
                    nombre_completo AS NombreCompleto,
                    correo_electronico AS CorreoElectronico,
                    contrasena_hash AS ContrasenaHash,
                    fecha_creacion AS FechaCreacion,
                    estado AS Estado
                FROM usuario
                WHERE correo_electronico = @correoElectronico;
            ";

            return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { correoElectronico });
        }

        // ================================
        // VALIDAR SI EXISTE CORREO
        // ================================
        public async Task<bool> ExisteCorreoAsync(string correoElectronico)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT COUNT(1)
                FROM usuario
                WHERE correo_electronico = @correoElectronico;
            ";

            var cantidad = await conexion.ExecuteScalarAsync<int>(sql, new { correoElectronico });

            return cantidad > 0;
        }

        // ================================
        // ACTUALIZAR USUARIO
        // ================================
        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE usuario
                SET
                    nombre_completo = @NombreCompleto,
                    correo_electronico = @CorreoElectronico,
                    contrasena_hash = @ContrasenaHash,
                    estado = @Estado
                WHERE id_usuario = @IdUsuario;
            ";

            var filas = await conexion.ExecuteAsync(sql, usuario);

            return filas > 0;
        }

        // ================================
        // LISTAR USUARIOS ACTIVOS
        // ================================
        public async Task<IEnumerable<Usuario>> ListarAsync()
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                SELECT 
                    id_usuario AS IdUsuario,
                    nombre_completo AS NombreCompleto,
                    correo_electronico AS CorreoElectronico,
                    contrasena_hash AS ContrasenaHash,
                    fecha_creacion AS FechaCreacion,
                    estado AS Estado
                FROM usuario
                WHERE estado = TRUE
                ORDER BY id_usuario;
            ";

            return await conexion.QueryAsync<Usuario>(sql);
        }

        // ================================
        // ELIMINAR (LÓGICO)
        // ================================
        public async Task<bool> EliminarAsync(int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE usuario
                SET estado = FALSE
                WHERE id_usuario = @idUsuario;
            ";

            var filas = await conexion.ExecuteAsync(sql, new { idUsuario });

            return filas > 0;
        }

        // ================================
        // REACTIVAR USUARIO
        // ================================
        public async Task<bool> ReactivarAsync(int idUsuario)
        {
            using var conexion = _fabricaConexion.CrearConexion();

            string sql = @"
                UPDATE usuario
                SET estado = TRUE
                WHERE id_usuario = @idUsuario;
            ";

            var filas = await conexion.ExecuteAsync(sql, new { idUsuario });

            return filas > 0;
        }
    }
}
