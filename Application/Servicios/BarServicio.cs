using MusicBares.Application.Interfaces.Servicios;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.DTOs.Bar;
using MusicBares.Entidades;

namespace MusicBares.Application.Servicios
{
    /// <summary>
    /// Implementación de la lógica de negocio para la entidad Bar.
    ///
    /// RESPONSABILIDADES:
    /// - Convertir DTOs a Entidades y viceversa
    /// - Aplicar reglas de negocio
    /// - Coordinar acceso a repositorios
    ///
    /// NO DEBE:
    /// - Ejecutar SQL
    /// - Conocer detalles de infraestructura
    /// </summary>
    public class BarServicio : IBarServicio
    {
        private readonly IBarRepositorio _barRepositorio;

        public BarServicio(IBarRepositorio barRepositorio)
        {
            _barRepositorio = barRepositorio;
        }

        /// <summary>
        /// Obtiene todos los bares activos del sistema.
        /// Convierte las entidades a DTOs de listado.
        /// </summary>
        public async Task<IEnumerable<BarListadoDto>> ObtenerTodosAsync()
        {
            // El repositorio devuelve entidades
            var bares = await _barRepositorio.ListarAsync();

            // Convertimos Entidad → DTO
            return bares.Select(bar => new BarListadoDto
            {
                IdBar = bar.IdBar,
                NombreBar = bar.NombreBar,
                Direccion = bar.Direccion,
                Estado = bar.Estado
            });
        }

        /// <summary>
        /// Crea un nuevo bar en el sistema.
        /// Convierte el DTO de entrada en una entidad persistible.
        /// </summary>
        public async Task<int> CrearAsync(BarCrearDto dto)
        {
            // Regla de negocio mínima de ejemplo
            // (aquí podrían ir validaciones más complejas)
            if (string.IsNullOrWhiteSpace(dto.NombreBar))
                throw new ArgumentException("El nombre del bar es obligatorio.");

            // Convertimos DTO → Entidad
            var bar = new Bar
            {
                NombreBar = dto.NombreBar,
                Direccion = dto.Direccion,
                IdUsuario = dto.IdUsuario,
                // Estos valores coinciden con defaults de la BD,
                // pero se dejan explícitos para claridad
                Estado = true,
                FechaRegistro = DateTime.UtcNow
            };

            // El repositorio devuelve el ID generado
            return await _barRepositorio.CrearAsync(bar);
        }
    }
}
