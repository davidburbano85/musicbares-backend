using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.Bar;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    /// <summary>
    /// Implementa la lógica de negocio para la gestión de bares.
    /// </summary>
    public class BarServicio : IBarServicio
    {
        private readonly IBarRepositorio _barRepositorio;

        public BarServicio(IBarRepositorio barRepositorio)
        {
            _barRepositorio = barRepositorio;
        }

        /// <summary>
        /// Crea un nuevo bar en el sistema.
        /// </summary>
        public async Task<BarRespuestaDto> CrearAsync(BarCrearDto dto)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrWhiteSpace(dto.NombreBar))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El nombre del bar es obligatorio"
                    };

                if (string.IsNullOrWhiteSpace(dto.Direccion))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "La dirección es obligatoria"
                    };

                // Mapping DTO → Entidad
                var bar = new Bar
                {
                    NombreBar = dto.NombreBar,
                    Direccion = dto.Direccion,
                    IdUsuario = dto.IdUsuario,
                    Estado = true
                };

                // Guardar en BD
                await _barRepositorio.CrearAsync(bar);

                return new BarRespuestaDto
                {
                    Exitoso = true,
                    Mensaje = "Bar creado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BarRespuestaDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al crear el bar: {ex.Message}"
                };
            }
        }
        /// <summary>
        /// Obtiene un bar específico por su identificador.
        /// </summary>
        public async Task<BarListadoDto?> ObtenerPorIdAsync(int idBar)
        {
            try
            {
                var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);

                if (bar == null)
                    return null;

                // Mapping Entidad → DTO
                return new BarListadoDto
                {
                    IdBar = bar.IdBar,
                    NombreBar = bar.NombreBar,
                    Direccion = bar.Direccion,
                    Estado = bar.Estado
                };
            }
            catch
            {
                // En consultas simples es válido devolver null
                return null;
            }
        }

        /// Lista todos los bares activos del sistema.
        /// Uso administrativo o listado general.
        public async Task<IEnumerable<BarListadoDto>> ListarAsync()
        {
            try
            {
                var bares = await _barRepositorio.ListarAsync();

                // Mapping Entidad → DTO
                return bares.Select(bar => new BarListadoDto
                {
                    IdBar = bar.IdBar,
                    NombreBar = bar.NombreBar,
                    Direccion = bar.Direccion,
                    Estado = bar.Estado
                });
            }
            catch
            {
                // Retornamos colección vacía para evitar romper el flujo
                return Enumerable.Empty<BarListadoDto>();
            }
        }

        /// <summary>
        /// Obtiene los bares pertenecientes a un usuario específico.
        /// Fundamental para arquitectura multi-tenant.
        /// </summary>
        public async Task<IEnumerable<BarListadoDto>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            try
            {
                var bares = await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario);

                // Mapping Entidad → DTO
                return bares.Select(bar => new BarListadoDto
                {
                    IdBar = bar.IdBar,
                    NombreBar = bar.NombreBar,
                    Direccion = bar.Direccion,
                    Estado = bar.Estado
                });
            }
            catch
            {
                return Enumerable.Empty<BarListadoDto>();
            }
        }

        /// <summary>
        /// Actualiza la información de un bar existente.
        /// </summary>
        public async Task<BarRespuestaDto> ActualizarAsync(BarActualizarDto dto)
        {
            try
            {
                // Verificar que el bar exista
                var barExistente = await _barRepositorio.ObtenerPorIdAsync(dto.IdBar);

                if (barExistente == null)
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El bar no existe"
                    };

                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(dto.NombreBar))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El nombre del bar es obligatorio"
                    };

                if (string.IsNullOrWhiteSpace(dto.Direccion))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "La dirección es obligatoria"
                    };

                // Mapping DTO → Entidad
                barExistente.NombreBar = dto.NombreBar;
                barExistente.Direccion = dto.Direccion;
                barExistente.Estado = dto.Estado;

                var actualizado = await _barRepositorio.ActualizarAsync(barExistente);

                if (!actualizado)
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "No se pudo actualizar el bar"
                    };

                return new BarRespuestaDto
                {
                    Exitoso = true,
                    Mensaje = "Bar actualizado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BarRespuestaDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al actualizar el bar: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Elimina lógicamente un bar cambiando su estado a inactivo.
        /// </summary>
        public async Task<BarRespuestaDto> EliminarAsync(int idBar)
        {
            try
            {
                // Verificar existencia
                var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);

                if (bar == null)
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El bar no existe"
                    };

                // Soft delete
                bar.Estado = false;

                var eliminado = await _barRepositorio.ActualizarAsync(bar);

                if (!eliminado)
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "No se pudo eliminar el bar"
                    };

                return new BarRespuestaDto
                {
                    Exitoso = true,
                    Mensaje = "Bar eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BarRespuestaDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al eliminar el bar: {ex.Message}"
                };
            }
        }
    }
}

    

