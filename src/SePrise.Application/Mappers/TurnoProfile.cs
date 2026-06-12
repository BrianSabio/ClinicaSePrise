namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Turno;

/// <summary>
/// Profile de AutoMapper para TurnoAggregate.
/// Mapea entre Turno (entidad) y DTOs de turno.
/// </summary>
public class TurnoProfile : Profile
{
    public TurnoProfile()
    {
        // TurnoAggregate → TurnoDTO (lectura)
        CreateMap<TurnoAggregate, TurnoDTO>()
            .ForMember(dest => dest.Estado, 
                opt => opt.MapFrom(src => src.Estado.ToString())) // enum → string
            .ForMember(dest => dest.PacienteNombre,
                opt => opt.MapFrom(src => src.Paciente != null ? src.Paciente.Nombre + " " + src.Paciente.Apellido : string.Empty))
            .ForMember(dest => dest.MedicoNombre,
                opt => opt.MapFrom(src => src.Medico != null ? src.Medico.Nombre + " " + src.Medico.Apellido : string.Empty))
            .ForMember(dest => dest.EspecialidadNombre,
                opt => opt.MapFrom(src => src.Especialidad != null ? src.Especialidad.Nombre : string.Empty));

        // TurnoCrearDTO → TurnoAggregate (creación)
        CreateMap<TurnoCrearDTO, TurnoAggregate>()
            .ConvertUsing((src, ctx) =>
                TurnoAggregate.Crear(
                    src.IdPaciente,
                    src.IdMedico,
                    src.IdEspecialidad,
                    src.IdSala,
                    src.FechaHoraInicio,
                    src.DuracionMinutos
                )
            );
    }
}
