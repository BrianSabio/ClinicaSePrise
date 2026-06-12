namespace SePrise.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using SePrise.Application.Validators.Paciente;
using SePrise.Application.Validators.Turno;
using SePrise.Application.Validators.Atencion;
using SePrise.Application.DTOs.Paciente;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.DTOs.Atencion;

/// <summary>
/// Extensión de IServiceCollection para registrar servicios de Application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los validadores de FluentValidation.
    /// </summary>
    public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PacienteCrearDTO>, PacienteCrearValidator>();
        services.AddScoped<IValidator<PacienteActualizarDTO>, PacienteActualizarValidator>();
        
        services.AddScoped<IValidator<TurnoCrearDTO>, TurnoCrearValidator>();
        services.AddScoped<IValidator<TurnoConfirmarDTO>, TurnoConfirmarValidator>();
        
        services.AddScoped<IValidator<AtencionCrearEspontaneaDTO>, AtencionCrearEspontaneaValidator>();
        services.AddScoped<IValidator<AtencionActualizarNotasDTO>, AtencionActualizarNotasValidator>();

        return services;
    }
}
