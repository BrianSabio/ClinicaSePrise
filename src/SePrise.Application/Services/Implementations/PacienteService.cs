namespace SePrise.Application.Services.Implementations;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;
using SePrise.Domain.Entities;
using SePrise.Application.DTOs.Paciente;
using SePrise.Application.Services.Interfaces;

/// <summary>
/// Servicio de aplicación para gestión de pacientes.
/// Maneja CRUD y búsquedas de pacientes.
/// </summary>
public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa una nueva instancia de PacienteService.
    /// </summary>
    public PacienteService(IPacienteRepository pacienteRepository, IMapper mapper)
    {
        _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Crea un nuevo paciente en el sistema.
    /// </summary>
    /// <param name="dto">Datos del paciente a crear.</param>
    /// <returns>DTO del paciente creado.</returns>
    /// <exception cref="ArgumentNullException">Si dto es null.</exception>
    /// <exception cref="PacienteException">Si DNI ya está registrado.</exception>
    public async Task<PacienteDTO> CrearPacienteAsync(PacienteCrearDTO dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Convertir DNI string a Value Object
        var dni = Dni.Crear(dto.DNI);

        // Verificar que DNI no existe
        var pacienteExistente = await _pacienteRepository.GetByDNIAsync(dni);
        if (pacienteExistente != null)
            throw new PacienteException($"Ya existe un paciente registrado con DNI {dni}");

        // Crear paciente
        var paciente = _mapper.Map<Paciente>(dto);

        // Persistir
        await _pacienteRepository.AddAsync(paciente);
        await _pacienteRepository.SaveChangesAsync();

        // Retornar DTO
        var idGenerado = paciente.IdPaciente;
        var pacienteCreado = await _pacienteRepository.GetByIdAsync(idGenerado);
        return _mapper.Map<PacienteDTO>(pacienteCreado);
    }

    /// <summary>
    /// Obtiene un paciente por su ID.
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    /// <returns>DTO del paciente.</returns>
    /// <exception cref="ArgumentException">Si idPaciente &lt;= 0.</exception>
    /// <exception cref="PacienteException">Si paciente no existe.</exception>
    public async Task<PacienteDTO> ObtenerPacienteAsync(int idPaciente)
    {
        if (idPaciente <= 0)
            throw new ArgumentException("ID de paciente debe ser > 0", nameof(idPaciente));

        var paciente = await _pacienteRepository.GetByIdAsync(idPaciente);
        if (paciente == null || !paciente.Activo)
            throw new PacienteException($"Paciente con ID {idPaciente} no encontrado o inactivo");

        return _mapper.Map<PacienteDTO>(paciente);
    }

    /// <summary>
    /// Obtiene un paciente por su DNI.
    /// Útil para búsquedas en recepción.
    /// </summary>
    /// <param name="dni">DNI del paciente (string).</param>
    /// <returns>DTO del paciente.</returns>
    /// <exception cref="ArgumentException">Si DNI es inválido.</exception>
    /// <exception cref="PacienteException">Si paciente no existe.</exception>
    public async Task<PacienteDTO> ObtenerPacientePorDNIAsync(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("DNI no puede estar vacío", nameof(dni));

        var dniValueObject = Dni.Crear(dni); // Lanza ArgumentException si es inválido

        var paciente = await _pacienteRepository.GetByDNIAsync(dniValueObject);
        if (paciente == null)
            throw new PacienteException($"Paciente con DNI {dni} no encontrado");

        return _mapper.Map<PacienteDTO>(paciente);
    }

    /// <summary>
    /// Obtiene todos los pacientes activos del sistema.
    /// </summary>
    /// <returns>Colección de DTOs de pacientes activos.</returns>
    public async Task<IEnumerable<PacienteDTO>> ListarPacientesActivosAsync()
    {
        var pacientes = await _pacienteRepository.GetAllActivosAsync();
        return _mapper.Map<IEnumerable<PacienteDTO>>(pacientes);
    }

    /// <summary>
    /// Actualiza los datos de un paciente existente.
    /// Solo actualiza propiedades suministradas en el DTO (null = no cambiar).
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    /// <param name="dto">Datos a actualizar.</param>
    /// <returns>DTO del paciente actualizado.</returns>
    /// <exception cref="ArgumentException">Si idPaciente &lt;= 0.</exception>
    /// <exception cref="ArgumentNullException">Si dto es null.</exception>
    /// <exception cref="PacienteException">Si paciente no existe.</exception>
    public async Task<PacienteDTO> ActualizarPacienteAsync(int idPaciente, PacienteActualizarDTO dto)
    {
        if (idPaciente <= 0)
            throw new ArgumentException("ID de paciente debe ser > 0", nameof(idPaciente));

        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var paciente = await _pacienteRepository.GetByIdAsync(idPaciente);
        if (paciente == null)
            throw new PacienteException($"Paciente con ID {idPaciente} no encontrado");

        // Utilizando el método en la entidad para actualización de propiedades
        paciente.ActualizarDatos(
            dto.DNI,
            dto.FechaNacimiento,
            dto.Genero,
            dto.Nombre, 
            dto.Apellido, 
            dto.Email, 
            dto.Telefono, 
            dto.Direccion, 
            dto.Ciudad, 
            dto.Provincia, 
            dto.CodigoPostal
        );
        
        await _pacienteRepository.UpdateAsync(paciente);
        await _pacienteRepository.SaveChangesAsync();

        var pacienteActualizado = await _pacienteRepository.GetByIdAsync(idPaciente);
        return _mapper.Map<PacienteDTO>(pacienteActualizado);
    }

    /// <summary>
    /// Desactiva un paciente (soft delete).
    /// El paciente no puede agendar nuevos turnos.
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    /// <exception cref="ArgumentException">Si idPaciente &lt;= 0.</exception>
    /// <exception cref="PacienteException">Si paciente no existe.</exception>
    public async Task DesactivarPacienteAsync(int idPaciente)
    {
        if (idPaciente <= 0)
            throw new ArgumentException("ID de paciente debe ser > 0", nameof(idPaciente));

        var paciente = await _pacienteRepository.GetByIdAsync(idPaciente);
        if (paciente == null)
            throw new PacienteException($"Paciente con ID {idPaciente} no encontrado");

        paciente.Desactivar();

        await _pacienteRepository.UpdateAsync(paciente);
        await _pacienteRepository.SaveChangesAsync();
    }
}
