namespace SePrise.API.Models.Responses;

/// <summary>
/// Modelo de respuesta HTTP con los datos de una sala o consultorio.
/// </summary>
public class SalaResponse
{
    /// <summary>Identificador único de la sala.</summary>
    public int Id { get; set; }

    /// <summary>Número o código de la sala (ej: "101", "Consultorio A").</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Tipo de sala: "Consultorio", "Procedimientos" o "Espera".</summary>
    public string TipoSala { get; set; } = string.Empty;
}
