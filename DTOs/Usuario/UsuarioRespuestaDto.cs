namespace MusicBares.DTOs.Usuario
{
    public class UsuarioRespuestaDto
    {
        // Indica si la operación fue exitosa
        public bool Exitoso { get; set; }

        // Mensaje informativo para el frontend
        public string Mensaje { get; set; } = string.Empty;

        // Id del usuario cuando aplique (crear/login)
        public int? IdUsuario { get; set; }


        // 🔹 Constructor vacío (necesario para serialización)
        public UsuarioRespuestaDto()
        {
        }

        // 🔹 Constructor para respuestas simples
        public UsuarioRespuestaDto(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        // 🔹 Constructor cuando se necesita devolver ID
        public UsuarioRespuestaDto(bool exitoso, string mensaje, int? idUsuario)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
            IdUsuario = idUsuario;
        }
    }
}
