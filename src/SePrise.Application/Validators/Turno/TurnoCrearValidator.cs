namespace SePrise.Application.Validators.Turno;

using FluentValidation;
using SePrise.Application.DTOs.Turno;
using System;

/// <summary>
/// Validador para el DTO de creación de turno.
/// </summary>
public class TurnoCrearValidator : AbstractValidator<TurnoCrearDTO>
{
    public TurnoCrearValidator()
    {
        RuleFor(t => t.IdPaciente)
            .GreaterThan(0).WithMessage("IdPaciente debe ser mayor a 0");

        RuleFor(t => t.IdMedico)
            .GreaterThan(0).WithMessage("IdMedico debe ser mayor a 0");

        RuleFor(t => t.IdEspecialidad)
            .GreaterThan(0).WithMessage("IdEspecialidad debe ser mayor a 0");

        RuleFor(t => t.IdSala)
            .GreaterThan(0).WithMessage("IdSala debe ser mayor a 0");

        RuleFor(t => t.FechaHoraInicio)
            .NotEmpty().WithMessage("FechaHoraInicio es requerida")
            .GreaterThanOrEqualTo(DateTime.Now.AddDays(1).Date)
            .WithMessage("Turno debe ser para mañana o posterior");

        RuleFor(t => t.DuracionMinutos)
            .GreaterThan(0).WithMessage("DuracionMinutos debe ser > 0")
            .LessThanOrEqualTo(480).WithMessage("DuracionMinutos no puede exceder 8 horas (480 minutos)");
    }
}
