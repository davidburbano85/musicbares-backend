using MusicBares.DTOs.Mesa;

namespace MusicBares.Application.Interfaces.Servicios
{
    // ======================================================
    // Interfaz del servicio de mesas
    // Contiene la lógica de negocio relacionada con Mesa
    // ======================================================
    public interface IMesaServicio
    {
        // ======================================================
        // Crea una nueva mesa dentro de un bar
        // - Valida que el bar exista
        // - Valida que no se repita el número de mesa en el bar
        // ======================================================
        Task<MesaRespuestaDto> CrearAsync(MesaCrearDto dto);

        // ======================================================
        // Obtiene una mesa por su identificador único
        // Retorna NULL si la mesa no existe
        // ======================================================
        Task<MesaRespuestaDto?> ObtenerPorIdAsync(int idMesa);

        // ======================================================
        // Obtiene todas las mesas pertenecientes a un bar específico
        // Fundamental para mantener el aislamiento multi-tenant
        // ======================================================
        Task<IEnumerable<MesaListadoDto>> ObtenerPorBarAsync(int idBar);

        // ======================================================
        // Obtiene una mesa a partir de su código QR
        // Flujo público (cliente)
        // ======================================================
        Task<MesaRespuestaDto?> ObtenerPorCodigoQRAsync(string CodigoQR);

        // ======================================================
        // Actualiza información o estado de una mesa
        // ======================================================
        Task<MesaRespuestaDto> ActualizarAsync(MesaActualizarDto dto);

       
    }
}
