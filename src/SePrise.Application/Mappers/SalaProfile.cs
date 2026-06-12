namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Entities;
using SePrise.Application.DTOs.Sala;
public class SalaProfile : Profile
{
    public SalaProfile()
    {
        // Sala → SalaDTO (lectura)
        CreateMap<Sala, SalaDTO>()
            .ForMember(dest => dest.TipoSala, 
                opt => opt.MapFrom(src => src.TipoSala.ToString())); // enum → string

        // SalaCrearDTO → Sala (creación)
        CreateMap<SalaCrearDTO, Sala>()
            .ConvertUsing((src, ctx) =>
            {
                var tipoSala = Enum.Parse<TipoSala>(src.TipoSala);
                return Sala.Crear(src.Numero, tipoSala);
            });
    }
}


