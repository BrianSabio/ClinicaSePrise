namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Entities;
using SePrise.Application.DTOs.Especialidad;
public class EspecialidadProfile : Profile
{
    public EspecialidadProfile()
    {
        // Especialidad → EspecialidadDTO (lectura)
        CreateMap<Especialidad, EspecialidadDTO>();

        // EspecialidadCrearDTO → Especialidad (creación)
        CreateMap<EspecialidadCrearDTO, Especialidad>()
            .ConvertUsing((src, ctx) =>
                Especialidad.Crear(
                    src.Nombre,
                    src.Descripcion,
                    src.DuracionMinutos,
                    src.PermiteMultiplesTurnos
                )
            );
    }
}


