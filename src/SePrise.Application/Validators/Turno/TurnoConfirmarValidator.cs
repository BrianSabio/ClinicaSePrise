namespace SePrise.Application.Validators.Turno;

using FluentValidation;
using SePrise.Application.DTOs.Turno;
public class TurnoConfirmarValidator : AbstractValidator<TurnoConfirmarDTO>
{
    public TurnoConfirmarValidator()
    {
        RuleFor(t => t.IdTurno)
            .GreaterThan(0).WithMessage("IdTurno debe ser mayor a 0");
    }
}


