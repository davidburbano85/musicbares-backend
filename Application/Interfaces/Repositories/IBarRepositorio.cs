using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories;

public interface IBarRepositorio
{
    // Inserta un nuevo bar en la base de datos
    // Retorna el Id generado del bar
    Task<int> CrearAsync(Bar bar);

    // Obtiene un bar por su identificador único
    Task<Bar?> ObtenerPorIdAsync(int idBar);

    // Obtiene todos los bares pertenecientes a un usuario específico
    // Es fundamental para la arquitectura multi-tenant
    Task<IEnumerable<Bar>> ObtenerPorUsuarioAsync(int idUsuario);

    // Verifica si un bar existe y pertenece al usuario
    // Ayuda a reforzar seguridad y validaciones
    Task<bool> ExisteBarUsuarioAsync(int idBar, int idUsuario);

    // Actualiza la información de un bar existente
    Task<bool> ActualizarAsync(Bar bar);

    // Lista todos los bares activos del sistema
    // Puede usarse para administración general
    Task<IEnumerable<Bar>> ListarAsync();
}
