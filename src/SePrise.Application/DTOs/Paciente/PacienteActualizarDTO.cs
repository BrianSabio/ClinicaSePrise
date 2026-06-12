namespace SePrise.Application.DTOs.Paciente;

/// <summary>
/// DTO para actualizar un paciente existente.
/// Incluye solo los campos que pueden ser modificados (PUT/PATCH request).
/// </summary>
public class PacienteActualizarDTO
{
    /// <summary>
    /// Documento Nacional de Identidad. Opcional.
    /// </summary>
    public string? DNI { get; set; }

    /// <summary>
    /// Fecha de nacimiento del paciente. Opcional.
    /// </summary>
    public DateTime? FechaNacimiento { get; set; }

    /// <summary>
    /// Género del paciente. Opcional.
    /// </summary>
    public char? Genero { get; set; }

    /// <summary>
    /// Nombre del paciente. Opcional (solo si se quiere actualizar).
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Apellido del paciente. Opcional.
    /// </summary>
    public string? Apellido { get; set; }

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
