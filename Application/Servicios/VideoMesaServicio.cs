using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.DTOs.VideoMesa;

public class VideoMesaServicio : IVideoMesaServicio
{
    private readonly IVideoMesaRepositorio _repositorio;

    public VideoMesaServicio(IVideoMesaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<VideoMesaRespuestaDto> CrearAsync(VideoMesaCrearDto dto)
        => throw new NotImplementedException();

    public Task<IEnumerable<VideoMesaListadoDto>> ObtenerPorMesaAsync(int idMesa)
        => throw new NotImplementedException();

    public Task<VideoMesaRespuestaDto?> ObtenerSiguienteAsync(int idBar)
        => throw new NotImplementedException();

    public Task<bool> EliminarAsync(int idVideo)
        => throw new NotImplementedException();
}
