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
                Console.WriteLine("===== INICIO CrearAsync =====");

                // 🔹 1️⃣ Obtener usuario actual desde JWT
                int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();
                Console.WriteLine($"[Paso 1] idUsuario obtenido: {idUsuario}");

                // 🔹 2️⃣ Obtener bar activo del usuario
                Console.WriteLine("[Paso 2] Obteniendo bar activo del usuario...");
                var bar = await _barRepositorio.ObtenerBarPorUsuarioIdAsync(idUsuario);

                if (bar == null)
                {
                    Console.WriteLine("[ERROR] No se encontró un bar activo para este usuario");
                    Console.WriteLine("===== FIN CrearAsync =====");
                    return new MesaRespuestaDto { Estado = false };
                }
                Console.WriteLine($"[Paso 2] Bar encontrado: IdBar={bar.IdBar}, NombreBar={bar.NombreBar}, Estado={bar.Estado}");

                // 🔹 3️⃣ Validar que el número de mesa no esté repetido
                Console.WriteLine($"[Paso 3] Validando si número de mesa {dto.NumeroMesa} ya existe en el bar...");
                bool numeroExiste = await _mesaRepositorio.ExisteNumeroMesaAsync(bar.IdBar, dto.NumeroMesa);

                if (numeroExiste)
                {
                    Console.WriteLine($"[ERROR] Número de mesa repetido: {dto.NumeroMesa}");
                    Console.WriteLine("===== FIN CrearAsync =====");
                    return new MesaRespuestaDto { Estado = false, IdBar = bar.IdBar };
                }
                Console.WriteLine("[Paso 3] Número de mesa libre");

                // 🔹 4️⃣ Crear objeto Mesa
                Console.WriteLine("[Paso 4] Preparando objeto Mesa...");
                var mesa = new Mesa
                {
                    NumeroMesa = dto.NumeroMesa,
                    IdBar = bar.IdBar,
                    CodigoQR = dto.CodigoQR,
                    Estado = true
                };
                Console.WriteLine($"[Paso 4] Mesa preparada: NumeroMesa={mesa.NumeroMesa}, CodigoQR={mesa.CodigoQR}, Estado={mesa.Estado}");

                // 🔹 5️⃣ Insertar mesa en DB
                Console.WriteLine("[Paso 5] Insertando mesa en la base de datos...");
                int idMesa = await _mesaRepositorio.CrearAsync(mesa);
                Console.WriteLine($"[Paso 5] Mesa creada con IdMesa={idMesa}");

                // 🔹 6️⃣ Retornar DTO final
                Console.WriteLine("[Paso 6] Retornando respuesta exitosa");
                Console.WriteLine("===== FIN CrearAsync =====\n");

                return new MesaRespuestaDto
                {
                    IdMesa = idMesa,
                    NumeroMesa = mesa.NumeroMesa,
                    IdBar = mesa.IdBar,
                    CodigoQR = mesa.CodigoQR,
                    Estado = mesa.Estado
                };
            }
            catch (Exception ex)
            {
                // 🔹 Captura de error completa
                Console.WriteLine("[EXCEPCIÓN] Ocurrió un error en CrearAsync");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException:");
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.InnerException.StackTrace);
                }

                Console.WriteLine("===== FIN CrearAsync =====\n");
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
            if (bar == null)
            {
                // En lugar de continuar y lanzar SQL inválida, retornamos null
                return null;
            }

            // Validar que la mesa pertenezca al bar del usuario
            bool existeMesaEnBar = await _mesaRepositorio.ExisteMesaBarAsync(idMesa, bar.IdBar);
            if (!existeMesaEnBar)
            {
                return null;
            }

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
