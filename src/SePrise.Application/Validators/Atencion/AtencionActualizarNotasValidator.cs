namespace SePrise.Application.Validators.Atencion;

using FluentValidation;
using SePrise.Application.DTOs.Atencion;
public class AtencionActualizarNotasValidator : AbstractValidator<AtencionActualizarNotasDTO>
{
    public AtencionActualizarNotasValidator()
    {
        RuleFor(a => a.IdAtencion)
            .GreaterThan(0).WithMessage("IdAtencion debe ser mayor a 0");

        RuleFor(a => a.Notas)
            .NotEmpty().WithMessage("Notas es requerida")
            .MinimumLength(5).WithMessage("Notas debe tener al menos 5 caracteres")
            .MaximumLength(1000).WithMessage("Notas no puede exceder 1000 caracteres");
    }
}


