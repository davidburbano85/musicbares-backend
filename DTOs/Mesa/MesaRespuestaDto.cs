namespace MusicBares.DTOs.Mesa
{
    // ======================================================
    // DTO: MesaRespuestaDto
    // Se utiliza como RESPUESTA del sistema
    // cuando se consulta o se crea una mesa
    // ======================================================
    public class MesaRespuestaDto
    {
        // ----------------------------------------------
        // Identificador único de la mesa
        // Se usa para operaciones futuras
        // ----------------------------------------------
        public int IdMesa { get; set; }

        // ----------------------------------------------
        // Número visible de la mesa dentro del bar
        // Ejemplo: Mesa 1, Mesa 8
        // ----------------------------------------------
        public int NumeroMesa { get; set; }

        // ----------------------------------------------
        // Bar al que pertenece la mesa
        // Refuerza el aislamiento multi-bar
        // ----------------------------------------------
        public int IdBar { get; set; }

        // ----------------------------------------------
        // Código QR asociado a la mesa
        // Permite acceso directo del cliente
        // ----------------------------------------------
        public string? CodigoQR { get; set; } 

        // ----------------------------------------------
        // Estado lógico de la mesa
        // TRUE  = disponible / activa
        // FALSE = deshabilitada
        // ----------------------------------------------
        public bool Estado { get; set; }

        // ----------------------------------------------
        // Fecha en la que se creó la mesa
        // Útil para auditoría o historial
        // ----------------------------------------------
        public DateTime FechaCreacion { get; set; }
    }
}
