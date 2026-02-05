using MusicBares.Entidades;

namespace MusicBares.Application.Interfaces.Repositories;

public interface IMesaRepositorio
{
    // Inserta una nueva mesa en un bar específico
    // Retorna el Id generado de la mesa
    Task<int> CrearAsync(Mesa mesa);

    // Obtiene una mesa por su identificador único
    Task<Mesa?> ObtenerPorIdAsync(int idMesa);

    // Obtiene todas las mesas pertenecientes a un bar
    // Fundamental para mantener el aislamiento por bar
    Task<IEnumerable<Mesa>> ObtenerPorBarAsync(int idBar);

    // Obtiene una mesa usando su código QR
    // Permite que clientes accedan directamente a la mesa
    Task<Mesa?> ObtenerPorCodigoQRAsync(string codigoQR);

    // Verifica si una mesa pertenece a un bar específico
    // Se usa para reforzar validaciones multi-tenant
    Task<bool> ExisteMesaBarAsync(int idMesa, int idBar);

    // Verifica si ya existe un número de mesa dentro del mismo bar
    // Evita duplicados como dos mesas con número 5 en el mismo bar
    Task<bool> ExisteNumeroMesaAsync(int idBar, int numeroMesa);

    // Actualiza la información o estado de una mesa
    Task<bool> ActualizarAsync(Mesa mesa);

    // Lista todas las mesas activas del sistema
    Task<IEnumerable<Mesa>> ListarAsync();
}
