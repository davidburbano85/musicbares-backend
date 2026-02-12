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
        // Servicio que permite obtener usuario autenticado desde JWT
        private readonly IUsuarioActualServicio _usuarioActualServicio;
        public BarServicio(IBarRepositorio barRepositorio,
            IUsuarioActualServicio usuarioActualServicio )
        {
            _barRepositorio = barRepositorio;
            _usuarioActualServicio = usuarioActualServicio;
        }

        /// Crea un nuevo bar en el sistema.
        public async Task<BarRespuestaDto> CrearAsync(BarCrearDto dto)
        {
            try
            {
                // ======================================================
                // VALIDACIONES BÁSICAS
                // ======================================================

                // Validamos que el nombre del bar no esté vacío
                if (string.IsNullOrWhiteSpace(dto.NombreBar))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El nombre del bar es obligatorio"
                    };

                // Validamos que la dirección no esté vacía
                if (string.IsNullOrWhiteSpace(dto.Direccion))
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "La dirección es obligatoria"
                    };

                // ======================================================
                // OBTENER USUARIO AUTENTICADO DESDE JWT
                // ======================================================

                // Ya NO confiamos en dto.IdUsuario (por seguridad)
                // Se obtiene directamente del token del usuario logueado
                int idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

                // ======================================================
                // VALIDAR REGLA: UN USUARIO SOLO PUEDE TENER UN BAR
                // ======================================================

                // Consultamos si el usuario ya tiene bar registrado
                var baresUsuario = await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario);

                if (baresUsuario.Any())
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "El usuario ya tiene un bar registrado"
                    };

                // ======================================================
                // MAPPING DTO → ENTIDAD
                // ======================================================

                var bar = new Bar
                {
                    NombreBar = dto.NombreBar,
                    Direccion = dto.Direccion,

                    // Se asigna el usuario autenticado automáticamente
                    IdUsuario = idUsuario,

                    Estado = true
                };

                // ======================================================
                // GUARDAR EN BASE DE DATOS
                // ======================================================

                await _barRepositorio.CrearAsync(bar);

                // ======================================================
                // RESPUESTA EXITOSA
                // ======================================================

                return new BarRespuestaDto
                {
                    Exitoso = true,
                    Mensaje = "Bar creado correctamente"
                };
            }
            catch (Exception ex)
            {
                // ======================================================
                // MANEJO DE ERRORES CONTROLADO
                // ======================================================

                return new BarRespuestaDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al crear el bar: {ex.Message}"
                };
            }
        }


        /// Obtiene un bar específico por su identificador.
        public async Task<BarListadoDto?> ObtenerPorIdAsync(int idBar)
        {
            try
            {
                var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);

                if (bar == null)
                    return null;

                // ⭐ VALIDAR PROPIETARIO
                var idUsuarioActual = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

                if (bar.IdUsuario != idUsuarioActual)
                    return null;

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
                return null;
            }
        }

        /// Lista todos los bares activos del sistema.
        /// Uso administrativo o listado general.
        public async Task<IEnumerable<BarListadoDto>> ListarAsync()
        {
            try
            {
                var idUsuario = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

                var bares = await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario);

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


        /// Obtiene los bares pertenecientes a un usuario específico.
        /// Fundamental para arquitectura multi-tenant.

        public async Task<IEnumerable<BarListadoDto>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            try
            {
                var idUsuarioActual = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

                if (idUsuario != idUsuarioActual)
                    return Enumerable.Empty<BarListadoDto>();

                var bares = await _barRepositorio.ObtenerPorUsuarioAsync(idUsuario);

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


        /// Actualiza la información de un bar existente.

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

                // ⭐ VALIDAR QUE EL BAR PERTENEZCA AL USUARIO AUTENTICADO
                var idUsuarioActual = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

                if (barExistente.IdUsuario != idUsuarioActual)
                {
                    return new BarRespuestaDto
                    {
                        Exitoso = false,
                        Mensaje = "No tienes permisos para modificar este bar"
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

            // ⭐ VALIDAR QUE EL BAR PERTENEZCA AL USUARIO AUTENTICADO
            var idUsuarioActual = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

            if (bar.IdUsuario != idUsuarioActual)
                throw new Exception("No tienes permisos para eliminar este bar");

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
            Console.WriteLine("🔥 VERSION NUEVA DEL SERVICIO 🔥🔥🔥");

            // Validación básica del id
            if (idBar <= 0)
                throw new ArgumentException("El id del bar es inválido");

            // Buscar el bar en BD
            Console.WriteLine("ANTES DE LLAMAR REPO");
            var bar = await _barRepositorio.ObtenerPorIdAsync(idBar);
            Console.WriteLine("DESPUES DE LLAMAR REPO");

            if (bar == null)
                throw new Exception("El bar no existe");

            // ⭐ VALIDAR QUE EL BAR PERTENEZCA AL USUARIO AUTENTICADO
            var idUsuarioActual = await _usuarioActualServicio.ObtenerIdUsuarioAsync();

            if (bar.IdUsuario != idUsuarioActual)
                throw new Exception("No tienes permisos para reactivar este bar");

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

    

