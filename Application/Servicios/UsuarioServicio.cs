using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Usuario;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

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
                if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                    return new UsuarioRespuestaDto(false, "El nombre es obligatorio");

                if (string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                    return new UsuarioRespuestaDto(false, "El correo es obligatorio");

                var correoExiste = await _usuarioRepositorio.ExisteCorreoAsync(dto.CorreoElectronico);

                if (correoExiste)
                    return new UsuarioRespuestaDto(false, "El correo ya está registrado");

                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    CorreoElectronico = dto.CorreoElectronico,
                    Estado = true,

                    // Se llenará automáticamente cuando llegue JWT Supabase
                    AuthUserId = Guid.Empty
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
        // LISTAR USUARIOS
        // ================================
        public async Task<IEnumerable<UsuarioListadoDto>> ListarAsync()
        {
            try
            {
                var usuarios = await _usuarioRepositorio.ListarAsync();

                return usuarios.Select(usuario => new UsuarioListadoDto
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    CorreoElectronico = usuario.CorreoElectronico,
                    Estado = usuario.Estado
                });
            }
            catch
            {
                return Enumerable.Empty<UsuarioListadoDto>();
            }
        }

        // ================================
        // OBTENER POR ID
        // ================================
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

        // ================================
        // ACTUALIZAR USUARIO
        // ================================
        public async Task<UsuarioRespuestaDto> ActualizarAsync(UsuarioActualizarDto dto)
        {
            try
            {
                var usuario = await ObtenerPorCorreoAsync(dto.CorreoElectronico);

                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "Usuario no encontrado.");

                // Cambiar correo si aplica
                if (!string.IsNullOrWhiteSpace(dto.CorreoElectronicoNuevo) &&
                    !dto.CorreoElectronicoNuevo.Equals(usuario.CorreoElectronico, StringComparison.OrdinalIgnoreCase))
                {
                    var correoExiste = await ObtenerPorCorreoAsync(dto.CorreoElectronicoNuevo);

                    if (correoExiste != null)
                        return new UsuarioRespuestaDto(false, "El correo electrónico ya está en uso.");

                    usuario.CorreoElectronico = dto.CorreoElectronicoNuevo;
                }

                usuario.NombreCompleto = dto.NombreCompleto;
                usuario.Estado = dto.Estado;

                var actualizado = await _usuarioRepositorio.ActualizarAsync(usuario);

                if (!actualizado)
                    return new UsuarioRespuestaDto(false, "No se pudo actualizar el usuario.");

                return new UsuarioRespuestaDto(true, "Usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return new UsuarioRespuestaDto(false, $"Error al actualizar usuario: {ex.Message}");
            }
        }

        // ================================
        // ELIMINAR
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
        // REACTIVAR
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

        // ================================
        // OBTENER POR CORREO
        // ================================
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
        {
            if (string.IsNullOrWhiteSpace(correoElectronico))
                return null;

            return await _usuarioRepositorio.ObtenerPorCorreoAsync(correoElectronico);
        }

        // ================================
        // OBTENER POR AUTH ID (SUPABASE)
        // ================================
        public async Task<Usuario?> ObtenerPorAuthIdAsync(Guid authUserId)
        {
            return await _usuarioRepositorio.ObtenerPorAuthIdAsync(authUserId);
        }
    }
}
