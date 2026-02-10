namespace MusicBares.Application.Interfaces.Context
{
    // Define contrato para obtener datos del usuario autenticado
    public interface IUsuarioContext
    {
        // Retorna el auth_user_id proveniente del JWT
        Guid ObtenerAuthUserId();
    }
}
