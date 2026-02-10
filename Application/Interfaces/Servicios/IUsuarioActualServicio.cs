using MusicBares.Entidades; // Permite usar la entidad Usuario y Bar

namespace MusicBares.Application.Interfaces.Servicios
{
    // Define contrato para obtener información del usuario autenticado actual
    public interface IUsuarioActualServicio
    {
        // Retorna el id_usuario del sistema interno
        Task<int> ObtenerIdUsuarioAsync();

        // Retorna el bar asociado al usuario autenticado
        Task<Bar> ObtenerBarAsync();

        // Retorna solo el id del bar asociado al usuario autenticado
        Task<int> ObtenerIdBarAsync();

        // Retorna la entidad completa del usuario autenticado
        Task<Usuario> ObtenerUsuarioAsync();
    }
}
