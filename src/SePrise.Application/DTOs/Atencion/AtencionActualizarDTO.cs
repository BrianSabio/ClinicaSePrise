namespace SePrise.Application.DTOs.Atencion;

/// <summary>
/// DTO para actualizar una atención existente.
/// Utilizado principalmente para agregar notas clínicas antes de finalizar la atención.
/// </summary>
public class AtencionActualizarDTO
{
    /// <summary>
    /// Notas clínicas o evoluciones agregadas por el médico.
    /// </summary>
    public string? Notas { get; set; }
}
