namespace SePrise.Application.Mappers;

using AutoMapper;
using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Paciente;
public class PacienteProfile : Profile
{
    public PacienteProfile()
    {
        // Paciente → PacienteDTO (lectura)
        CreateMap<Paciente, PacienteDTO>()
            .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.DNI.Valor)); // Value Object → string

        // PacienteCrearDTO → Paciente (creación)
        // Requiere factory method, así que usamos MapAction
        CreateMap<PacienteCrearDTO, Paciente>()
            .ConvertUsing((src, ctx) =>
            {
                var dni = Dni.Crear(src.DNI);
                return Paciente.Crear(
                    dni,
                    src.Nombre,
                    src.Apellido,
                    src.FechaNacimiento,
                    src.Genero,
                    src.Email,
                    src.Telefono,
                    src.Direccion,
                    src.Ciudad,
                    src.Provincia,
                    src.CodigoPostal
                );
            });

        // PacienteActualizarDTO → Paciente (actualización - solo propiedades específicas)
        // Nota: Actualización es más compleja, usualmente en servicio
        CreateMap<PacienteActualizarDTO, Paciente>(MemberList.Source)
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}


