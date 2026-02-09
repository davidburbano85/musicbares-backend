using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Mesa;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    // ======================================================
    // Implementación concreta del servicio de Mesa
    // Aquí vive la LÓGICA DE NEGOCIO
    // ======================================================
    public class MesaServicio : IMesaServicio
    {
        // ----------------------------------------------
        // Repositorio de mesas (acceso a BD)
        // ----------------------------------------------
        private readonly IMesaRepositorio _mesaRepositorio;

        // ----------------------------------------------
        // Constructor con inyección de dependencias
        // ASP.NET se encarga de pasar el repositorio
        // ----------------------------------------------
        public MesaServicio(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        // ======================================================
        // CREAR MESA
        // ======================================================
        public async Task<MesaRespuestaDto> CrearAsync(MesaCrearDto dto)
        {
            // ----------------------------------------------
            // 1. Validar datos mínimos
            // ----------------------------------------------
            if (dto.NumeroMesa <= 0)
                throw new Exception("El número de mesa debe ser mayor a cero.");

            // ----------------------------------------------
            // 2. Verificar que no exista el mismo número
            //    de mesa dentro del mismo bar
            // ----------------------------------------------
            bool existeNumero = await _mesaRepositorio
                .ExisteNumeroMesaAsync(dto.IdBar, dto.NumeroMesa);

            if (existeNumero)
                throw new Exception("Ya existe una mesa con ese número en el bar.");

            // ----------------------------------------------
            // 3. Crear la entidad Mesa
            //    (modelo de dominio)
            // ----------------------------------------------
            var mesa = new Mesa
            {
                NumeroMesa = dto.NumeroMesa,
                IdBar = dto.IdBar,
                CodigoQR = dto.CodigoQR,
                Estado = true
                // FechaCreacion normalmente la pone la BD
            };

            // ----------------------------------------------
            // 4. Guardar en base de datos
            // ----------------------------------------------
            int idGenerado = await _mesaRepositorio.CrearAsync(mesa);

            // ----------------------------------------------
            // 5. Retornar DTO de respuesta
            // ----------------------------------------------
            return new MesaRespuestaDto
            {
                IdMesa = idGenerado,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado,
                FechaCreacion = DateTime.UtcNow
            };
        }

        // ======================================================
        // OBTENER MESA POR ID
        // ======================================================
        public async Task<MesaRespuestaDto?> ObtenerPorIdAsync(int idMesa)
        {
            // ----------------------------------------------
            // 1. Consultar repositorio
            // ----------------------------------------------
            var mesa = await _mesaRepositorio.ObtenerPorIdAsync(idMesa);

            // ----------------------------------------------
            // 2. Si no existe, retornar null
            // ----------------------------------------------
            if (mesa == null)
                return null;

            // ----------------------------------------------
            // 3. Mapear entidad → DTO
            // ----------------------------------------------
            return new MesaRespuestaDto
            {
                IdMesa = mesa.IdMesa,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado,
            };
        }

        // ======================================================
        // OBTENER MESAS POR BAR
        // ======================================================
        public async Task<IEnumerable<MesaListadoDto>> ObtenerPorBarAsync(int idBar)
        {
            // ----------------------------------------------
            // 1. Obtener mesas del repositorio
            // ----------------------------------------------
            var mesas = await _mesaRepositorio.ObtenerPorBarAsync(idBar);

            // ----------------------------------------------
            // 2. Mapear cada entidad a DTO de listado
            // ----------------------------------------------
            return mesas.Select(m => new MesaListadoDto
            {
                IdMesa = m.IdMesa,
                NumeroMesa = m.NumeroMesa,
                Estado = m.Estado
            });
        }

        // ======================================================
        // OBTENER MESA POR CÓDIGO QR
        // ======================================================
        public async Task<MesaRespuestaDto?> ObtenerPorCodigoQRAsync(string codigoQR)
        {
            // ----------------------------------------------
            // 1. Buscar mesa por QR
            // ----------------------------------------------
            var mesa = await _mesaRepositorio.ObtenerPorCodigoQRAsync(codigoQR);

            // ----------------------------------------------
            // 2. Validar existencia
            // ----------------------------------------------
            if (mesa == null)
                return null;

            // ----------------------------------------------
            // 3. Mapear y retornar
            // ----------------------------------------------
            return new MesaRespuestaDto
            {
                IdMesa = mesa.IdMesa,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado,
            };
        }

        // ======================================================
        // ACTUALIZAR MESA
        // ======================================================
        public async Task<MesaRespuestaDto> ActualizarAsync(MesaActualizarDto dto)
        {
            // ----------------------------------------------
            // 1. Obtener mesa existente
            // ----------------------------------------------
            var mesa = await _mesaRepositorio.ObtenerPorIdAsync(dto.IdMesa);

            if (mesa == null)
                throw new Exception("La mesa no existe.");

            // ----------------------------------------------
            // 2. Actualizar campos permitidos
            // ----------------------------------------------
            mesa.NumeroMesa = dto.NumeroMesa;
            mesa.Estado = dto.Estado;

            // ----------------------------------------------
            // 3. Guardar cambios
            // ----------------------------------------------
            await _mesaRepositorio.ActualizarAsync(mesa);

            // ----------------------------------------------
            // 4. Retornar respuesta
            // ----------------------------------------------
            return new MesaRespuestaDto
            {
                IdMesa = mesa.IdMesa,
                NumeroMesa = mesa.NumeroMesa,
                IdBar = mesa.IdBar,
                CodigoQR = mesa.CodigoQR,
                Estado = mesa.Estado,
            };
        }

        // ======================================================
        // LISTAR TODAS LAS MESAS ACTIVAS
        // ======================================================
        public async Task<IEnumerable<MesaListadoDto>> ListarAsync()
        {
            var mesas = await _mesaRepositorio.ListarAsync();

            return mesas.Select(m => new MesaListadoDto
            {
                IdMesa = m.IdMesa,
                NumeroMesa = m.NumeroMesa,
                Estado = m.Estado,
                IdBar= m.IdBar,
                CodigoQR= m.CodigoQR,
            });
        }
    }
}
