using System.Security.Claims; // Permite leer claims del JWT
using MusicBares.Application.Interfaces.Context;

namespace MusicBares.Infrastructure.Context
{
    public class UsuarioContext : IUsuarioContext
    {
        // IHttpContextAccessor permite acceder al contexto HTTP actual
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Inyectamos IHttpContextAccessor
        public UsuarioContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Obtiene el auth_user_id desde el JWT
        public Guid ObtenerAuthUserId()
        {
            // Obtiene el usuario autenticado desde el contexto HTTP
            var usuario = _httpContextAccessor.HttpContext?.User;

            // Busca el claim "sub" dentro del token JWT
            var sub = usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? usuario?.FindFirst("sub")?.Value;

            // Si no existe el claim se lanza error
            if (string.IsNullOrEmpty(sub))
                throw new UnauthorizedAccessException("Token inválido");

            // Convierte el valor a Guid
            return Guid.Parse(sub);
        }
    }
}
