namespace SePrise.Application.DTOs.Paciente;

/// <summary>
/// DTO para crear un paciente nuevo.
/// Solo incluye los campos necesarios para crear (POST request).
/// </summary>
public class PacienteCrearDTO
{
    /// <summary>
    /// DNI del paciente (string, será convertido a Value Object).
    /// Obligatorio. Formato: 7-9 dígitos.
    /// </summary>
    public string DNI { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del paciente. Obligatorio. Max 100 caracteres.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del paciente. Obligatorio. Max 100 caracteres.
    /// </summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento. Obligatorio. Debe ser anterior a hoy.
    /// </summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Género: M, F u O. Obligatorio.
    /// </summary>
    public char Genero { get; set; }

    /// <summary>
    /// Email del paciente. Opcional.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del paciente. Opcional.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección del paciente. Opcional.
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad. Opcional.
    /// </summary>
    public string? Ciudad { get; set; }

    /// <summary>
    /// Provincia. Opcional.
    /// </summary>
    public string? Provincia { get; set; }

    /// <summary>
    /// Código postal. Opcional.
    /// </summary>
    public string? CodigoPostal { get; set; }
}
