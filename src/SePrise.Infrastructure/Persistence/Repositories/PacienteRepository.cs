using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;
using SePrise.Domain.ValueObjects;

namespace SePrise.Infrastructure.Persistence.Repositories;
public class PacienteRepository : IPacienteRepository
{
    // Contexto de EF Core inyectado por el contenedor de dependencias
    private readonly SePriseDbContext _context;
    public PacienteRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<Paciente?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del paciente debe ser mayor a 0.", nameof(id));

        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.IdPaciente == id);
    }
    public async Task<Paciente?> GetByDNIAsync(Dni dni)
    {
        if (dni is null)
            throw new ArgumentNullException(nameof(dni));

        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.DNI == dni);
    }
    public async Task<IEnumerable<Paciente>> GetAllActivosAsync()
    {
        // Solo pacientes activos, ordenados alfabéticamente por apellido y nombre
        return await _context.Pacientes
            .Where(p => p.Activo)
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }
    public async Task<bool> ExistsByDNIAsync(Dni dni)
    {
        if (dni is null)
            throw new ArgumentNullException(nameof(dni));

        return await _context.Pacientes
            .AnyAsync(p => p.DNI == dni);
    }
    public async Task<int> AddAsync(Paciente paciente)
    {
        if (paciente is null)
            throw new ArgumentNullException(nameof(paciente));

        // AddAsync registra la entidad como "Added" en el Change Tracker de EF
        await _context.Pacientes.AddAsync(paciente);

        // El ID se genera en la BD; está disponible después de SaveChangesAsync
        return paciente.IdPaciente;
    }
    public Task UpdateAsync(Paciente paciente)
    {
        if (paciente is null)
            throw new ArgumentNullException(nameof(paciente));

        // EF Core marca la entidad como "Modified" en el Change Tracker
        _context.Pacientes.Update(paciente);

        // No es una operación I/O, retornamos Task completado
        return Task.CompletedTask;
    }
    public async Task SaveChangesAsync()
    {
        // Persiste todos los cambios registrados en el Change Tracker (Unit of Work)
        await _context.SaveChangesAsync();
    }
}


