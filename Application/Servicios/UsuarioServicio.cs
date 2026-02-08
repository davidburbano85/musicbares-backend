using BCrypt.Net;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.DTOs.Usuario;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    public class UsuarioServicio
    {
        // Repositorio de usuarios para acceso a datos
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        // Inyección de dependencias
        public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // ================================
        // CREAR USUARIO
        // ================================
        public async Task<UsuarioRespuestaDto> CrearAsync(UsuarioCrearDto dto)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                    return new UsuarioRespuestaDto(false, "El nombre es obligatorio");

                // Validar correo
                if (string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                    return new UsuarioRespuestaDto(false, "El correo es obligatorio");

                // Validar contraseña
                if (string.IsNullOrWhiteSpace(dto.Contrasena))
                    return new UsuarioRespuestaDto(false, "La contraseña es obligatoria");

                // Verificar si el correo ya existe
                var correoExiste = await _usuarioRepositorio.ExisteCorreoAsync(dto.CorreoElectronico);

                if (correoExiste)
                    return new UsuarioRespuestaDto(false, "El correo ya está registrado");

                // Hashear contraseña con BCrypt
                var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

                // Mapear DTO → Entidad
                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    CorreoElectronico = dto.CorreoElectronico,
                    ContrasenaHash = hash,
                    Estado = true
                };

                var id = await _usuarioRepositorio.CrearAsync(usuario);

                return new UsuarioRespuestaDto(true, "Usuario creado correctamente", id);
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error al crear usuario: {ex.Message}");
            }
        }

        // ================================
        // LOGIN USUARIO
        // ================================
        public async Task<UsuarioRespuestaDto> LoginAsync(UsuarioLoginDto dto)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(dto.CorreoElectronico);

                // Validar existencia
                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "Credenciales inválidas");

                // Validar estado
                if (!usuario.Estado)
                    return new UsuarioRespuestaDto(false, "El usuario está inactivo");

                // Validar contraseña
                bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);

                if (!passwordValido)
                    return new UsuarioRespuestaDto(false, "Credenciales inválidas");

                return new UsuarioRespuestaDto(true, "Login exitoso", usuario.IdUsuario);
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error en login: {ex.Message}");
            }
        }

        // ================================
        // LISTAR USUARIOS ACTIVOS
        // ================================
        public async Task<IEnumerable<UsuarioListadoDto>> ListarAsync()
        {
            var usuarios = await _usuarioRepositorio.ListarAsync();

            // Convertimos entidades a DTO de listado
            return usuarios.Select(u => new UsuarioListadoDto
            {
                IdUsuario = u.IdUsuario,
                NombreCompleto = u.NombreCompleto,
                CorreoElectronico = u.CorreoElectronico,
                Estado = u.Estado
            });
        }

        // ================================
        // ACTUALIZAR USUARIO
        // ================================
        public async Task<UsuarioRespuestaDto> ActualizarAsync(UsuarioActualizarDto dto)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(dto.IdUsuario);

                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "El usuario no existe");

                // Validaciones
                if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                    return new UsuarioRespuestaDto(false, "El nombre es obligatorio");

                if (string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                    return new UsuarioRespuestaDto(false, "El correo es obligatorio");

                usuario.NombreCompleto = dto.NombreCompleto;
                usuario.CorreoElectronico = dto.CorreoElectronico;
                usuario.Estado = dto.Estado;

                // Si envían contraseña nueva, se vuelve a hashear
                if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                {
                    usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
                }

                var actualizado = await _usuarioRepositorio.ActualizarAsync(usuario);

                if (!actualizado)
                    return new UsuarioRespuestaDto(false, "No se pudo actualizar el usuario");

                return new UsuarioRespuestaDto(true, "Usuario actualizado correctamente");
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error al actualizar usuario: {ex.Message}");
            }
        }

        // ================================
        // ELIMINAR USUARIO (SOFT DELETE)
        // ================================
        public async Task<UsuarioRespuestaDto> EliminarAsync(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                    return new UsuarioRespuestaDto(false, "Id inválido");

                var existe = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);

                if (existe == null)
                    return new UsuarioRespuestaDto(false, "El usuario no existe");

                var eliminado = await _usuarioRepositorio.EliminarAsync(idUsuario);

                if (!eliminado)
                    return new UsuarioRespuestaDto(false, "No se pudo eliminar el usuario");

                return new UsuarioRespuestaDto(true, "Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error al eliminar usuario: {ex.Message}");
            }
        }

        // ================================
        // REACTIVAR USUARIO
        // ================================
        public async Task<UsuarioRespuestaDto> ReactivarAsync(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                    return new UsuarioRespuestaDto(false, "Id inválido");

                var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);

                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "El usuario no existe");

                if (usuario.Estado)
                    return new UsuarioRespuestaDto(false, "El usuario ya está activo");

                var reactivado = await _usuarioRepositorio.ReactivarAsync(idUsuario);

                if (!reactivado)
                    return new UsuarioRespuestaDto(false, "No se pudo reactivar el usuario");

                return new UsuarioRespuestaDto(true, "Usuario reactivado correctamente");
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error al reactivar usuario: {ex.Message}");
            }
        }


        public async Task<UsuarioListadoDto?> ObtenerPorIdAsync(int idUsuario)
        {
            if (idUsuario <= 0)
                return null;

            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
                return null;

            return new UsuarioListadoDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                CorreoElectronico = usuario.CorreoElectronico,
                Estado = usuario.Estado
            };
        }




    }
}
