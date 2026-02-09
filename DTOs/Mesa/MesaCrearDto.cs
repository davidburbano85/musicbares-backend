namespace MusicBares.DTOs.Mesa
{
    // ======================================================
    // DTO: MesaCrearDto
    // Se utiliza cuando se va a CREAR una nueva mesa
    // ======================================================
    public class MesaCrearDto
    {
        // ----------------------------------------------
        // Identificador del bar al que pertenece la mesa
        // Viene del frontend o del contexto del usuario
        // ----------------------------------------------
        public int IdBar { get; set; }

        // ----------------------------------------------
        // Número visible de la mesa (ej: Mesa 1, Mesa 5)
        // Debe ser único dentro del mismo bar
        // ----------------------------------------------
        public int NumeroMesa { get; set; }

        // ----------------------------------------------
        // Código QR asociado a la mesa
        // Puede ser:
        // - null (si se genera después)
        // - string con URL / hash
        // ----------------------------------------------
        public string? CodigoQR { get; set; }

        // ----------------------------------------------
        // Estado inicial de la mesa
        // TRUE  = mesa activa
        // FALSE = mesa deshabilitada
        // Normalmente se crea en TRUE
        // ----------------------------------------------
        public bool Estado { get; set; } = true;
    }
}
