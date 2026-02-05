namespace MusicBares.DTOs.Bar
{
    public class BarListadoDto
    {
        public int IdBar { get; set; }
        public string NombreBar { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
