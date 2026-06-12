using System.Text.Json.Serialization;

namespace SePrise.WinForms.Models;

public class PacienteDTO
{
    [JsonPropertyName("id")]
    public int IdPaciente { get; set; }
    public string DNI { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class CreatePacienteRequest
{
    public string DNI { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
}

public class UpdatePacienteRequest
{
    public string DNI { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
}

public class TurnoDTO
{
    [JsonPropertyName("id")]
    public int IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public int IdSala { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    // Campo requerido por la API (se incluye para evitar JsonException en modo estricto)
    public DateTime? FechaCreacion { get; set; }
    public DateTime? FechaUltimaModificacion { get; set; }
}

public class CreateTurnoRequest
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public int IdSala { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
}

public class ReprogramarTurnoRequest
{
    public DateTime NuevaFechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
}

public class MedicoDTO
{
    [JsonPropertyName("id")]
    public int IdMedico { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public List<EspecialidadDTO> Especialidades { get; set; } = new();
}

public class EspecialidadDTO
{
    [JsonPropertyName("id")]
    public int IdEspecialidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionMinutos { get; set; }
}

public class SalaDTO
{
    [JsonPropertyName("id")]
    public int IdSala { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string TipoSala { get; set; } = string.Empty;
}

public class AtencionDTO
{
    [JsonPropertyName("id")]
    public int IdAtencion { get; set; }
    public int? IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaHoraAcreditacion { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string Notas { get; set; } = string.Empty;
    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    // Campos requeridos por la API (se incluyen para evitar JsonException en modo estricto)
    public DateTime? FechaCreacion { get; set; }
    public DateTime? FechaUltimaModificacion { get; set; }
}

public class AcreditarPacienteRequest
{
    public int IdTurno { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}

public class CrearDemandaEspontaneaRequest
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}

public class FinalizarAtencionRequest
{
    public string Notas { get; set; } = string.Empty;
}


