namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Atencion;
public class AtencionProfile : Profile
{
    public AtencionProfile()
    {
        // AtencionAggregate → AtencionDTO (lectura)
        CreateMap<AtencionAggregate, AtencionDTO>()
            .ForMember(dest => dest.Estado, 
                opt => opt.MapFrom(src => src.Estado.ToString())) // enum → string
            .ForMember(dest => dest.ModalidadPago, 
                opt => opt.MapFrom(src => src.ModalidadPago.ToString())) // enum → string
            .ForMember(dest => dest.PacienteNombre,
                opt => opt.MapFrom(src => src.Paciente != null ? src.Paciente.Nombre + " " + src.Paciente.Apellido : string.Empty))
            .ForMember(dest => dest.MedicoNombre,
                opt => opt.MapFrom(src => src.Medico != null ? src.Medico.Nombre + " " + src.Medico.Apellido : string.Empty))
            .ForMember(dest => dest.EspecialidadNombre,
                opt => opt.MapFrom(src => src.Turno != null && src.Turno.Especialidad != null 
                    ? src.Turno.Especialidad.Nombre 
                    : "Demanda Espontánea"));

        // AtencionCrearDTO → AtencionAggregate (creación desde turno confirmado)
        CreateMap<AtencionCrearDTO, AtencionAggregate>()
            .ConvertUsing((src, ctx) =>
            {
                var modalidad = Enum.Parse<ModalidadPago>(src.ModalidadPago);
                return AtencionAggregate.CrearDesdeConfirmacion(
                    src.IdTurno,
                    src.IdPaciente,
                    src.IdMedico,
                    modalidad
                );
            });

        // AtencionCrearEspontaneaDTO → AtencionAggregate (creación sin turno)
        CreateMap<AtencionCrearEspontaneaDTO, AtencionAggregate>()
            .ConvertUsing((src, ctx) =>
            {
                var modalidad = Enum.Parse<ModalidadPago>(src.ModalidadPago);
                return AtencionAggregate.CrearDemandaEspontanea(
                    src.IdPaciente,
                    src.IdMedico,
                    modalidad
                );
            });
    }
}


