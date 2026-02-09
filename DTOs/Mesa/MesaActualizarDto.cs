namespace MusicBares.DTOs.Mesa
{
    // ======================================================
    // DTO: MesaActualizarDto
    // Se usa cuando se ACTUALIZA una mesa existente
    // ======================================================
    public class MesaActualizarDto
    {
        // ----------------------------------------------
        // Identificador único de la mesa
        // Es obligatorio para saber qué registro actualizar
        // ----------------------------------------------
        public int IdMesa { get; set; }

        // ----------------------------------------------
        // Número de la mesa (puede cambiarse)
        // Debe seguir siendo único dentro del bar
        // La validación se hace en el Servicio
        // ----------------------------------------------
        public int NumeroMesa { get; set; }

        // ----------------------------------------------
        // Código QR asociado a la mesa
        // Puede actualizarse (ej: regenerar QR)
        // ----------------------------------------------
        public string? CodigoQr { get; set; }

        // ----------------------------------------------
        // Estado de la mesa
        // TRUE  = activa
        // FALSE = inactiva
        // Se usa para soft delete o deshabilitar mesas
        // ----------------------------------------------
        public bool Estado { get; set; }
    }
}

