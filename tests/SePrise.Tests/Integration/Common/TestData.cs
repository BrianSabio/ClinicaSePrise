namespace SePrise.Tests.Integration.Common;

using System;
using SePrise.Application.DTOs.Paciente;
using SePrise.API.Models.Requests;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.DTOs.Atencion;

/// <summary>
/// Provee métodos factory para crear datos válidos de prueba (Request DTOs).
/// </summary>
public static class TestData
{
    public static CreatePacienteRequest ValidPacienteRequest(string? dni = null)
    {
        return new CreatePacienteRequest
        {
            DNI = dni ?? TestDefaults.DefaultDni,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@test.com",
            FechaNacimiento = DateTime.UtcNow.AddYears(-30).Date,
            Genero = "M",
            Telefono = "1122334455",
            Direccion = "Calle 123",
            Ciudad = "Buenos Aires",
            Provincia = "Buenos Aires",
            CodigoPostal = "1000"
        };
    }

    public static CreateMedicoRequest ValidMedicoRequest(string? matricula = null)
    {
        return new CreateMedicoRequest
        {
            NumeroMatricula = matricula ?? TestDefaults.DefaultMatricula,
            Nombre = "Carlos",
            Apellido = "García",
            Email = "carlos@test.com",
            Telefono = "1199887766"
        };
    }

    public static CreateEspecialidadRequest ValidEspecialidadRequest(string? nombre = null)
    {
        return new CreateEspecialidadRequest
        {
            Nombre = nombre ?? TestDefaults.DefaultEspecialidadNombre,
            Descripcion = "Consultas generales",
            DuracionMinutos = TestDefaults.DefaultDuracionMinutos,
            PermiteMultiplesTurnos = false
        };
    }

    public static CreateSalaRequest ValidSalaRequest(string? numero = null)
    {
        return new CreateSalaRequest
        {
            Numero = numero ?? TestDefaults.DefaultSalaNumero,
            TipoSala = "Consultorio"
        };
    }
}
