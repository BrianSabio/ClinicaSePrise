namespace SePrise.Application.DTOs.Paciente;

/// <summary>
/// DTO de lectura completa de paciente.
/// Contiene toda la información del paciente (respuesta GET).
/// </summary>
public class PacienteDTO
{
    /// <summary>
    /// Identificador único del paciente.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// DNI del paciente (string, ya convertido desde Value Object).
    /// </summary>
    public string DNI { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del paciente.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del paciente.
    /// </summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento (solo fecha, sin hora).
    /// </summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Género: M, F u O.
    /// </summary>
    public char Genero { get; set; }

    /// <summary>
    /// Email del paciente (nullable).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Teléfono del paciente (nullable).
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección del paciente (nullable).
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad (nullable).
    /// </summary>
    public string? Ciudad { get; set; }

    /// <summary>
    /// Provincia (nullable).
    /// </summary>
    public string? Provincia { get; set; }

    /// <summary>
    /// Código postal (nullable).
    /// </summary>
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Indica si el paciente está activo.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de registro en el sistema.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
