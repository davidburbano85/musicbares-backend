using MusicBares.Application.Interfaces.Servicios; // Permite implementar la interfaz del servicio
using MusicBares.Application.Interfaces.Context;   // Permite usar IUsuarioContext para leer el JWT
using MusicBares.Application.Interfaces.Repositories; // Permite usar repositorios
using MusicBares.Entidades; // Permite usar entidades Usuario y Bar

namespace MusicBares.Application.Servicios
{
    // Servicio que obtiene información del usuario autenticado actual
    public class UsuarioActualServicio : IUsuarioActualServicio
    {
        // Permite obtener auth_user_id desde el JWT
        private readonly IUsuarioContext _usuarioContext;

        // Permite consultar usuarios en base de datos
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        // Permite consultar bares en base de datos
        private readonly IBarRepositorio _barRepositorio;

        // Cache del usuario durante el request
        private Usuario? _usuarioCache;

        // Cache del bar durante el request
        private Bar? _barCache;

        // Constructor con inyección de dependencias
        public UsuarioActualServicio(
            IUsuarioContext usuarioContext,
            IUsuarioRepositorio usuarioRepositorio,
            IBarRepositorio barRepositorio)
        {
            _usuarioContext = usuarioContext;
            _usuarioRepositorio = usuarioRepositorio;
            _barRepositorio = barRepositorio;
        }

        // Obtiene la entidad completa del usuario autenticado
        public async Task<Usuario> ObtenerUsuarioAsync()
        {
            // Si ya fue consultado durante el request se retorna cache
            if (_usuarioCache != null)
                return _usuarioCache;

            // Obtiene auth_user_id desde JWT
            var authUserId = _usuarioContext.ObtenerAuthUserId();

            // Busca usuario en base de datos
            var usuario = await _usuarioRepositorio.ObtenerPorAuthIdAsync(authUserId);

            // Si no existe usuario se lanza excepción
            if (usuario == null)
                throw new Exception("El usuario autenticado no existe en el sistema");

            // Guarda en cache
            _usuarioCache = usuario;

            return usuario;
        }

        // Obtiene solo el id_usuario interno
        public async Task<int> ObtenerIdUsuarioAsync()
        {
            // Usa el método anterior para evitar repetir lógica
            var usuario = await ObtenerUsuarioAsync();

            return usuario.IdUsuario;
        }

        // Obtiene el bar asociado al usuario autenticado
        public async Task<Bar> ObtenerBarAsync()
        {
            // Si ya fue consultado durante el request se retorna cache
            if (_barCache != null)
                return _barCache;

            // Obtiene id_usuario interno
            var idUsuario = await ObtenerIdUsuarioAsync();

            // Busca bar del usuario
            var bar = await _barRepositorio.ObtenerBarPorUsuarioIdAsync(idUsuario);

            // Si no existe bar se lanza excepción
            if (bar == null)
                throw new Exception("El usuario no tiene un bar asociado");

            // Guarda en cache
            _barCache = bar;

            return bar;
        }

        // Obtiene solo el id_bar asociado al usuario autenticado
        public async Task<int> ObtenerIdBarAsync()
        {
            // Usa el método anterior para evitar repetir lógica
            var bar = await ObtenerBarAsync();

            return bar.IdBar;
        }
    }
}
