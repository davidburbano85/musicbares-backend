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
                // Validar existencia del bar
                var barExistente = await _barRepositorio.ObtenerPorIdAsync(dto.IdBar);

                if (barExistente == null)
                {
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El bar no existe"
                    };
                }

                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(dto.NombreBar))
                {
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El nombre del bar es obligatorio"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.Direccion))
                {
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "La dirección es obligatoria"
                    };
                }

                // Mapping DTO → Entidad
                barExistente.NombreBar = dto.NombreBar.Trim();
                barExistente.Direccion = dto.Direccion.Trim();

                // ⚠️ Estado NO se modifica aquí
                // Estado solo se cambia en Eliminar o Reactivar

                var actualizado = await _barRepositorio.ActualizarAsync(barExistente);

                if (!actualizado)
                {
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "No se pudo actualizar el bar"
                    };
                }

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
            // Validación básica
            if (idBar <= 0)
                throw new ArgumentException("El id del bar es inválido");

            // Verificamos si el bar existe
            var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);

            if (bar == null)
                throw new Exception("El bar no existe");

            // Ejecutamos eliminación lógica
            var eliminado = await _barRepositorio.EliminarAsync(idBar);

            if (!eliminado)
                throw new Exception("No fue posible eliminar el bar");

            // Respuesta estándar del sistema
            return new BarRespuestaDto
            {
                Exitoso = true,
                Mensaje = "Bar eliminado correctamente"
            };
        }

        public async Task<BarRespuestaDto> ReactivarAsync(int idBar)
        {
            Console.WriteLine("🔥 ENTRO AL SERVICIO");


            // Validación básica del id
            if (idBar <= 0)
                throw new ArgumentException("El id del bar es inválido");

            // Buscar el bar en BD
            var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);

            if (bar == null)
                throw new Exception("El bar no existe");

            // Si ya está activo, no hacemos nada
            if (bar.Estado)
                throw new Exception("El bar ya se encuentra activo");

            // Reactivar el bar
            bar.Estado = true;

            // Usamos el método de actualización existente
            var actualizado = await _barRepositorio.ActualizarAsync(bar);

            if (!actualizado)
                throw new Exception("No fue posible reactivar el bar");

            // Respuesta estándar del sistema
            return new BarRespuestaDto
            {
                Exitoso = true,
                Mensaje = "Bar reactivado correctamente"
            };
        }

    }
}

    

