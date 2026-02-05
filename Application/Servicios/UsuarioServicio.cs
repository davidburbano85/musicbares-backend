using BCrypt.Net;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Usuario;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios;

public class UsuarioServicio : IUsuarioServicio
{
    // Repositorio para acceso a datos de usuarios
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    // Constructor con inyección de dependencias
    public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    // Registra un nuevo usuario aplicando reglas de negocio y seguridad
    public async Task<UsuarioRespuestaDto> RegistrarAsync(UsuarioCrearDto dto)
    {
        // Validar si el correo ya está registrado
        var correoExiste = await _usuarioRepositorio.ExisteCorreoAsync(dto.CorreoElectronico);

        if (correoExiste)
            throw new Exception("El correo electrónico ya está registrado");

        // Generar hash seguro de la contraseña
        string hashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

        // Crear entidad de dominio
        var usuario = new Usuario
        {
            NombreCompleto = dto.NombreCompleto,
            CorreoElectronico = dto.CorreoElectronico,
            ContrasenaHash = hashContrasena,
            FechaCreacion = DateTime.UtcNow,
            Estado = true
        };

        // Guardar usuario en base de datos
        int idGenerado = await _usuarioRepositorio.CrearAsync(usuario);

        // Construir respuesta segura
        return new UsuarioRespuestaDto
        {
            IdUsuario = idGenerado,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            FechaCreacion = usuario.FechaCreacion,
            Estado = usuario.Estado
        };
    }

    // Permite autenticación de usuario
    public async Task<UsuarioRespuestaDto?> LoginAsync(UsuarioLoginDto dto)
    {
        // Buscar usuario por correo
        var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(dto.CorreoElectronico);

        if (usuario == null)
            return null;

        // Verificar contraseña usando hash
        bool contrasenaValida = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);

        if (!contrasenaValida)
            return null;

        // Retornar información segura del usuario
        return new UsuarioRespuestaDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            FechaCreacion = usuario.FechaCreacion,
            Estado = usuario.Estado
        };
    }

    // Obtiene un usuario por Id
    public async Task<UsuarioRespuestaDto?> ObtenerPorIdAsync(int idUsuario)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);

        if (usuario == null)
            return null;

        return new UsuarioRespuestaDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            FechaCreacion = usuario.FechaCreacion,
            Estado = usuario.Estado
        };
    }

    // Permite actualizar información del usuario
    public async Task<bool> ActualizarAsync(UsuarioActualizarDto dto)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(dto.IdUsuario);

        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        // Aplicar cambios permitidos
        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.Estado = dto.Estado;

        return await _usuarioRepositorio.ActualizarAsync(usuario);
    }

    // Lista todos los usuarios activos
    public async Task<IEnumerable<UsuarioListadoDto>> ListarAsync()
    {
        var usuarios = await _usuarioRepositorio.ListarAsync();

        return usuarios.Select(u => new UsuarioListadoDto
        {
            IdUsuario = u.IdUsuario,
            NombreCompleto = u.NombreCompleto,
            CorreoElectronico = u.CorreoElectronico,
            Estado = u.Estado
        });
    }
}
