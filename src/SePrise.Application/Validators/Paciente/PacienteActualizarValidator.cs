namespace SePrise.Application.Validators.Paciente;

using FluentValidation;
using SePrise.Application.DTOs.Paciente;

/// <summary>
/// Validador para el DTO de actualización de paciente.
/// Todas las reglas son opcionales (can be null).
/// </summary>
public class PacienteActualizarValidator : AbstractValidator<PacienteActualizarDTO>
{
    public PacienteActualizarValidator()
    {
        RuleFor(p => p.Nombre)
            .MinimumLength(2).When(p => !string.IsNullOrEmpty(p.Nombre))
            .WithMessage("Nombre debe tener al menos 2 caracteres")
            .MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Nombre))
            .WithMessage("Nombre no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").When(p => !string.IsNullOrEmpty(p.Nombre))
            .WithMessage("Nombre solo puede contener letras y espacios");

        RuleFor(p => p.Email)
            .EmailAddress().When(p => !string.IsNullOrEmpty(p.Email))
            .WithMessage("Email debe ser un formato válido");

        RuleFor(p => p.Telefono)
            .MinimumLength(7).When(p => !string.IsNullOrEmpty(p.Telefono))
            .WithMessage("Teléfono debe tener al menos 7 caracteres");

        RuleFor(p => p.Direccion)
            .MaximumLength(200).When(p => !string.IsNullOrEmpty(p.Direccion))
            .WithMessage("Dirección no puede exceder 200 caracteres");
    }
}
