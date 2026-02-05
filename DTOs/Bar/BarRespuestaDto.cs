namespace MusicBares.DTOs.Bar
{
    /// <summary>
    /// DTO genérico de respuesta para operaciones sobre Bar.
    /// </summary>
    public class BarRespuestaDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
