namespace SePrise.Application.Validators.Paciente;

using FluentValidation;
using SePrise.Application.DTOs.Paciente;
using System;
using System.Linq;

/// <summary>
/// Validador para el DTO de creación de paciente.
/// Asegura que los datos básicos son válidos antes de persistir.
/// </summary>
public class PacienteCrearValidator : AbstractValidator<PacienteCrearDTO>
{
    public PacienteCrearValidator()
    {
        RuleFor(p => p.DNI)
            .NotEmpty().WithMessage("DNI es requerido")
            .Must(BeValidDNI).WithMessage("DNI debe ser un número válido (8-9 dígitos o con formato)");

        RuleFor(p => p.Nombre)
            .NotEmpty().WithMessage("Nombre es requerido")
            .MinimumLength(2).WithMessage("Nombre debe tener al menos 2 caracteres")
            .MaximumLength(50).WithMessage("Nombre no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("Nombre solo puede contener letras y espacios");

        RuleFor(p => p.Apellido)
            .NotEmpty().WithMessage("Apellido es requerido")
            .MinimumLength(2).WithMessage("Apellido debe tener al menos 2 caracteres")
            .MaximumLength(50).WithMessage("Apellido no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("Apellido solo puede contener letras y espacios");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email debe ser un formato válido");

        RuleFor(p => p.FechaNacimiento)
            .NotEmpty().WithMessage("Fecha de nacimiento es requerida")
            .LessThan(d => DateTime.Now).WithMessage("Fecha de nacimiento debe ser menor a hoy")
            .GreaterThan(d => new DateTime(1920, 1, 1)).WithMessage("Fecha de nacimiento debe ser posterior a 1920");

        RuleFor(p => p.Genero)
            .NotEmpty().WithMessage("Género es requerido")
            .Must(g => g == 'M' || g == 'F' || g == 'O').WithMessage("Género debe ser M, F u O");

        RuleFor(p => p.Telefono)
            .MinimumLength(7).When(p => !string.IsNullOrEmpty(p.Telefono))
            .WithMessage("Teléfono debe tener al menos 7 caracteres");

        RuleFor(p => p.Direccion)
            .MaximumLength(200).When(p => !string.IsNullOrEmpty(p.Direccion))
            .WithMessage("Dirección no puede exceder 200 caracteres");
    }

    private static bool BeValidDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            return false;

        // Remover guiones/espacios
        var clean = dni.Replace("-", "").Replace(".", "").Replace(" ", "");

        // Debe ser números de 7-9 dígitos
        return clean.Length >= 7 && clean.Length <= 9 && clean.All(char.IsDigit);
    }
}
