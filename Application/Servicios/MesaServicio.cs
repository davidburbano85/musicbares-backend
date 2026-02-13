using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Mesa;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    // Servicio de lógica de negocio para Mesas (versión segura)
    public class MesaServicio : IMesaServicio
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IBarRepositorio _barRepositorio;
        private readonly IUsuarioActualServicio _usuarioActualServicio;

        public MesaServicio(
            IMesaRepositorio mesaRepositorio,
            IBarRepositorio barRepositorio,
            IUsuarioActualServicio usuarioActualServicio)
        {
            _mesaRepositorio = mesaRepositorio;
            _barRepositorio = barRepositorio;
            _usuarioActualServicio = usuarioActualServicio;
        }

        // ==============================
        // CREAR MESA
        // ==============================
        public async Task<MesaRespuestaDto> CrearAsync(MesaCrearDto dto)
        {
            try
            {
                int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
                var bar = (await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario)).FirstOrDefault();

                if (bar == null)
                    return new MesaRespuestaDto { Estado = false };

                // Validar número repetido
                bool numeroExiste = await _mesaRepositorio.ExisteNumeroMesaAsync(bar.IdBar, dto.NumeroMesa);
                if (numeroExiste)
                    return new MesaRespuestaDto { Estado = false, IdBar = bar.IdBar };

                var mesa = new Mesa
                {
                    NumeroMesa = dto.NumeroMesa,
                    IdBar = bar.IdBar,
                    CodigoQR = dto.CodigoQR,
                    Estado = true
                };

                int idMesa = await _mesaRepositorio.CrearAsync(mesa);

                return new MesaRespuestaDto
                {
                    IdMesa = idMesa,
                    NumeroMesa = mesa.NumeroMesa,
                    IdBar = mesa.IdBar,
                    CodigoQR = mesa.CodigoQR,
                    Estado = mesa.Estado
                };
            }
            catch
            {
                return new MesaRespuestaDto { Estado = false };
            }
        }

        // ==============================
        // OBTENER MESA POR ID
        // ==============================
        public async Task<MesaRespuestaDto?> ObtenerPorIdAsync(int idMesa)
        {
            var mesa = await _mesaRepositorio.ObtenerPorIdAsync(idMesa);
            if (mesa == null)
                return null;

            // Validar que la mesa pertenezca al bar del usuario
            int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
            var bar = (await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario)).FirstOrDefault();
            if (bar == null || !(await _mesaRepositorio.ExisteMesaBarAsync(idMesa, bar.IdBar)))
                return null;

            return new MesaRespuestaDto
            {
                IdMesa = mesa.IdMesa,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado
            };
        }

        // ==============================
        // OBTENER MESAS POR BAR
        // ==============================
        public async Task<IEnumerable<MesaListadoDto>> ObtenerPorBarAsync(int unusedIdBar)
        {
            // Ignoramos el parámetro externo
            int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
            var bar = (await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario)).FirstOrDefault();
            if (bar == null)
                return Enumerable.Empty<MesaListadoDto>();

            var mesas = await _mesaRepositorio.ObtenerPorBarAsync(bar.IdBar);

            return mesas.Select(m => new MesaListadoDto
            {
                IdMesa = m.IdMesa,
                NumeroMesa = m.NumeroMesa,
                IdBar = m.IdBar,
                CodigoQR = m.CodigoQR,
                Estado = m.Estado
            });
        }

        // ==============================
        // OBTENER MESA POR CÓDIGO QR
        // ==============================
        public async Task<MesaRespuestaDto?> ObtenerPorCodigoQRAsync(string codigoQR)
        {
            var mesa = await _mesaRepositorio.ObtenerPorCodigoQRAsync(codigoQR);
            if (mesa == null)
                return null;

            return new MesaRespuestaDto
            {
                IdMesa = mesa.IdMesa,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado
            };
        }

        // ==============================
        // ACTUALIZAR MESA
        // ==============================
        public async Task<MesaRespuestaDto> ActualizarAsync(MesaActualizarDto dto)
        {
            try
            {
                var mesa = await _mesaRepositorio.ObtenerPorIdAsync(dto.IdMesa);
                if (mesa == null)
                    return new MesaRespuestaDto { Estado = false };

                // Validar que la mesa pertenezca al bar del usuario
                int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
                var bar = (await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario)).FirstOrDefault();
                if (bar == null || !(await _mesaRepositorio.ExisteMesaBarAsync(dto.IdMesa, bar.IdBar)))
                    return new MesaRespuestaDto { Estado = false };

                // Actualizar datos
                mesa.NumeroMesa = dto.NumeroMesa;
                mesa.CodigoQR = dto.CodigoQR;
                mesa.Estado = dto.Estado;

                bool actualizado = await _mesaRepositorio.ActualizarAsync(mesa);
                if (!actualizado)
                    return new MesaRespuestaDto { Estado = false };

                return new MesaRespuestaDto
                {
                    IdMesa = mesa.IdMesa,
                    NumeroMesa = mesa.NumeroMesa,
                    IdBar = mesa.IdBar,
                    CodigoQR = mesa.CodigoQR,
                    Estado = mesa.Estado
                };
            }
            catch
            {
                return new MesaRespuestaDto { Estado = false };
            }
        }
    }
}
