namespace MusicBares.DTOs.Mesa
{
    // ======================================================
    // DTO: MesaListadoDto
    // Se utiliza para LISTAR mesas
    // (ej: todas las mesas de un bar)
    // ======================================================
    public class MesaListadoDto
    {
        // ----------------------------------------------
        // Identificador único de la mesa
        // Usado para acciones (editar, eliminar, ver)
        // ----------------------------------------------
        public int IdMesa { get; set; }

        // ----------------------------------------------
        // Número visible de la mesa
        // Ejemplo: Mesa 1, Mesa 10
        // ----------------------------------------------
        public int NumeroMesa { get; set; }

        // ----------------------------------------------
        // Identificador del bar al que pertenece
        // Útil si el frontend maneja múltiples bares
        // ----------------------------------------------
        public int IdBar { get; set; }

        // ----------------------------------------------
        // Estado actual de la mesa
        // TRUE  = activa
        // FALSE = inactiva
        // ----------------------------------------------
        public bool Estado { get; set; }

        // ----------------------------------------------
        // Código QR de la mesa
        // Generalmente se muestra como link o botón
        // ----------------------------------------------
        public string? CodigoQr { get; set; }
    }
}
