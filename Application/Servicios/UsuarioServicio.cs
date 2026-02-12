using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Usuario;
using MusicBares.Entidades;
using BCrypt.Net;

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

                if (string.IsNullOrWhiteSpace(dto.Contrasena))
                    return new UsuarioRespuestaDto(false, "La contraseña es obligatoria");

                var correoExiste = await _usuarioRepositorio.ExisteCorreoAsync(dto.CorreoElectronico);

                if (correoExiste)
                    return new UsuarioRespuestaDto(false, "El correo ya está registrado");

                var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    CorreoElectronico = dto.CorreoElectronico,
                    ContrasenaHash = hash,
                    Estado = true,

                    // 🔥 IMPORTANTE:
                    // AuthUserId se llenará luego desde Supabase JWT
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
        // LOGIN
        // ================================
        public async Task<UsuarioRespuestaDto> LoginAsync(UsuarioLoginDto dto)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(dto.CorreoElectronico);

                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "Credenciales inválidas");

                if (!usuario.Estado)
                    return new UsuarioRespuestaDto(false, "El usuario está inactivo");

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
        // LISTAR USUARIOS
        // ================================
        public async Task<IEnumerable<UsuarioListadoDto>> ListarAsync()
        {          

            try
            {
                var usuarios = await _usuarioRepositorio.ListarAsync();

                // Mapping Entidad → DTO
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
                // Retornamos colección vacía para evitar romper el flujo
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

       
        public async Task<UsuarioRespuestaDto> ActualizarAsync(UsuarioActualizarDto dto)
        {
            try
            {
                // 1. Buscar por correo ACTUAL
                var usuario = await ObtenerPorCorreoAsync(dto.CorreoElectronico);

                if (usuario == null)
                    return new UsuarioRespuestaDto(false, "Usuario no encontrado.");

                // 2. Verificar contraseña SOLO si existe hash local
                if (!string.IsNullOrWhiteSpace(usuario.ContrasenaHash))
                {
                    if (string.IsNullOrWhiteSpace(dto.Contrasena))
                        return new UsuarioRespuestaDto(false, "Debe enviar la contraseña actual.");

                    if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash))
                        return new UsuarioRespuestaDto(false, "Contraseña incorrecta.");
                }
                // 3. Si quiere cambiar el correo, validar que no exista
                if (!string.IsNullOrWhiteSpace(dto.CorreoElectronicoNuevo) &&
                    !dto.CorreoElectronicoNuevo.Equals(usuario.CorreoElectronico, StringComparison.OrdinalIgnoreCase))
                {
                    var correoExiste = await ObtenerPorCorreoAsync(dto.CorreoElectronicoNuevo);
                    if (correoExiste != null)
                        return new UsuarioRespuestaDto(false, "El correo electrónico ya está en uso.");

                    usuario.CorreoElectronico = dto.CorreoElectronicoNuevo;
                }

                // 4. Actualizar otros campos
                usuario.NombreCompleto = dto.NombreCompleto;
                usuario.Estado = dto.Estado;

                // 5. Cambiar contraseña solo si viene una nueva
                if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                    usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

                // 6. Guardar cambios
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
        // ELIMINAR (SOFT DELETE)
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

        // Método para obtener un usuario por correo
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
        {
            if (string.IsNullOrWhiteSpace(correoElectronico))
                return null;

            // Llama al repositorio que hace la consulta a la base de datos
            var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(correoElectronico);

            return usuario;
        }

        // Obtiene usuario usando auth_user_id
        public async Task<Usuario?> ObtenerPorAuthIdAsync(Guid authUserId)
        {
            // Llama al repositorio para obtener el usuario
            return await _usuarioRepositorio.ObtenerPorAuthIdAsync(authUserId);
        }

    }
}
