namespace SePrise.Application.Validators.Atencion;

using FluentValidation;
using SePrise.Application.DTOs.Atencion;
public class AtencionCrearEspontaneaValidator : AbstractValidator<AtencionCrearEspontaneaDTO>
{
    public AtencionCrearEspontaneaValidator()
    {
        RuleFor(a => a.IdPaciente)
            .GreaterThan(0).WithMessage("IdPaciente debe ser mayor a 0");

        RuleFor(a => a.IdMedico)
            .GreaterThan(0).WithMessage("IdMedico debe ser mayor a 0");

        RuleFor(a => a.ModalidadPago)
            .NotEmpty().WithMessage("ModalidadPago es requerida")
            .Must(m => m == "ObraSocial" || m == "Particular")
            .WithMessage("ModalidadPago debe ser 'ObraSocial' o 'Particular'");
    }
}


