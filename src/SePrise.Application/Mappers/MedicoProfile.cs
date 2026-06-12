namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Entities;
using SePrise.Application.DTOs.Medico;

/// <summary>
/// Profile de AutoMapper para Medico.
/// </summary>
public class MedicoProfile : Profile
{
    public MedicoProfile()
    {
        // Medico → MedicoDTO (lectura)
        CreateMap<Medico, MedicoDTO>();

        // MedicoCrearDTO → Medico (creación)
        CreateMap<MedicoCrearDTO, Medico>()
            .ConvertUsing((src, ctx) =>
                Medico.Crear(
                    src.NumeroMatricula,
                    src.Nombre,
                    src.Apellido,
                    src.Email,
                    src.Telefono
                )
            );
    }
}
