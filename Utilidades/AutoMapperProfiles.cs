using AutoMapper;
using WebApiCanciones.DTOs;
using WebApiCanciones.Entidades;

namespace WebApiAlumnosSeg.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CancionDTO, Cancion>();
            CreateMap<Cancion, GetCancionDTO>();
        }
    }
}
