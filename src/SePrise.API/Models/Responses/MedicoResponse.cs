namespace SePrise.API.Models.Responses;

/// <summary>
/// Modelo de respuesta HTTP con los datos de un médico del sistema.
/// Incluye la lista de especialidades asociadas para el filtrado en el Frontend.
/// </summary>
public class MedicoResponse
{
    /// <summary>Identificador único del médico.</summary>
    public int Id { get; set; }

    /// <summary>Número de matrícula profesional.</summary>
    public string NumeroMatricula { get; set; } = string.Empty;

    /// <summary>Nombre del médico.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del médico.</summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Lista de especialidades asociadas al médico.
    /// Permite al Frontend filtrar médicos por especialidad seleccionada.
    /// </summary>
    public List<EspecialidadResponse> Especialidades { get; set; } = new();
}
