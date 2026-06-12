namespace SePrise.API.Mappers;

using AutoMapper;
using SePrise.Application.DTOs.Paciente;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.DTOs.Especialidad;
using SePrise.Application.DTOs.Medico;
using SePrise.Application.DTOs.Sala;
using SePrise.Domain.Entities;
using SePrise.API.Models.Requests;
using SePrise.API.Models.Responses;

public class ApiProfile : Profile
{
    public ApiProfile()
    {
        // ─── Requests → DTOs de Aplicación ───────────────────────────────────
        CreateMap<CreatePacienteRequest, PacienteCrearDTO>();
        CreateMap<UpdatePacienteRequest, PacienteActualizarDTO>();

        CreateMap<CreateTurnoRequest, TurnoCrearDTO>();
        CreateMap<ConfirmTurnoRequest, TurnoConfirmarDTO>();
        CreateMap<CancelTurnoRequest, TurnoCancelarDTO>();
        CreateMap<RescheduleTurnoRequest, TurnoReprogramarDTO>();

        CreateMap<CrearDemandaEspontaneaRequest, AtencionCrearEspontaneaDTO>();
        CreateMap<ActualizarNotasAtencionRequest, AtencionActualizarNotasDTO>();

        // ─── DTOs de Aplicación → Responses HTTP ─────────────────────────────
        // Paciente
        CreateMap<PacienteDTO, PacienteResponse>(MemberList.Source)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdPaciente))
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => src.FechaCreacion));

        // Turno — Se omite FechaUltimaModificacion para mantener contrato con Frontend
        CreateMap<TurnoDTO, TurnoResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdTurno))
            .ForMember(dest => dest.FechaUltimaModificacion, opt => opt.Ignore());

        // Atencion — Se omite FechaUltimaModificacion para mantener contrato con Frontend
        CreateMap<AtencionDTO, AtencionResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdAtencion))
            .ForMember(dest => dest.FechaUltimaModificacion, opt => opt.Ignore());

        // Reportes
        CreateMap<SePrise.Application.DTOs.Reportes.ReporteSummaryDTO, ReporteSummaryResponse>(MemberList.Source);

        // ─── Entidades de Dominio → Responses HTTP ───────────────────────────
        // Especialidad
        CreateMap<Especialidad, EspecialidadResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdEspecialidad));

        // Sala — TipoSala es enum, se convierte a string
        CreateMap<Sala, SalaResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdSala))
            .ForMember(dest => dest.TipoSala, opt => opt.MapFrom(src => src.TipoSala.ToString()));

        // Médico — Las especialidades se cargan por separado en el controller
        CreateMap<Medico, MedicoResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdMedico))
            .ForMember(dest => dest.Especialidades, opt => opt.Ignore()); // Se puebla manualmente

        // ─── DTOs de Aplicación → Responses HTTP (Especialidad anidada) ──────
        CreateMap<EspecialidadDTO, EspecialidadResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdEspecialidad));
    }
}


