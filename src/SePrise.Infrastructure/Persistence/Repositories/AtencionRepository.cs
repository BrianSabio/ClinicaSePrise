using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Repositories;
using SePrise.Domain.ValueObjects;

namespace SePrise.Infrastructure.Persistence.Repositories;
public class AtencionRepository : IAtencionRepository
{
    private readonly SePriseDbContext _context;
    public AtencionRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<AtencionAggregate?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID de la atención debe ser mayor a 0.", nameof(id));

        // Carga turno, paciente y médico para la lógica de negocio completa
        return await _context.Atenciones
            .Include(a => a.Turno)
            .Include(a => a.Paciente)
            .Include(a => a.Medico)
            .FirstOrDefaultAsync(a => a.IdAtencion == id);
    }
    public async Task<AtencionAggregate?> GetByTurnoAsync(int idTurno)
    {
        if (idTurno <= 0)
            throw new ArgumentException("El ID del turno debe ser mayor a 0.", nameof(idTurno));

        // La relación es 0..1: puede no existir atención para el turno (retorna null)
        return await _context.Atenciones
            .Include(a => a.Paciente)
            .Include(a => a.Medico)
            .FirstOrDefaultAsync(a => a.IdTurno == idTurno);
    }
    public async Task<IEnumerable<AtencionAggregate>> GetByPacienteAsync(int idPaciente)
    {
        if (idPaciente <= 0)
            throw new ArgumentException("El ID del paciente debe ser mayor a 0.", nameof(idPaciente));

        // Historial completo del paciente, del más reciente al más antiguo
        return await _context.Atenciones
            .Where(a => a.IdPaciente == idPaciente)
            .Include(a => a.Turno)
            .Include(a => a.Medico)
            .OrderByDescending(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<IEnumerable<AtencionAggregate>> GetByMedicoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        // Todas las atenciones del médico, del más reciente al más antiguo
        return await _context.Atenciones
            .Where(a => a.IdMedico == idMedico)
            .Include(a => a.Paciente)
            .Include(a => a.Turno)
            .OrderByDescending(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<IEnumerable<AtencionAggregate>> GetAtendiendoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        // Lista de pacientes actualmente siendo atendidos por el médico (sala de espera activa)
        return await _context.Atenciones
            .Where(a => a.IdMedico == idMedico
                     && a.Estado == EstadoAtencion.EnProgreso)
            .Include(a => a.Paciente)
            .OrderBy(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<IEnumerable<AtencionAggregate>> GetFinalizadasAsync(
        DateTime fechaDesde,
        DateTime fechaHasta)
    {
        if (fechaDesde > fechaHasta)
            throw new ArgumentException("FechaDesde no puede ser posterior a FechaHasta.");

        // Incluye turno, paciente y médico para reportes completos
        return await _context.Atenciones
            .Where(a => a.Estado == EstadoAtencion.Finalizada
                     && a.FechaHoraAcreditacion >= fechaDesde
                     && a.FechaHoraAcreditacion <= fechaHasta)
            .Include(a => a.Turno)
            .Include(a => a.Paciente)
            .Include(a => a.Medico)
            .OrderBy(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<IEnumerable<AtencionAggregate>> GetByEstadoAsync(EstadoAtencion estado)
    {
        return await _context.Atenciones
            .Where(a => a.Estado == estado)
            .Include(a => a.Paciente)
            .Include(a => a.Medico)
            .Include(a => a.Turno)
                .ThenInclude(t => t!.Especialidad)
            .OrderByDescending(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<IEnumerable<AtencionAggregate>> GetAllAsync()
    {
        return await _context.Atenciones
            .Include(a => a.Paciente)
            .Include(a => a.Medico)
            .Include(a => a.Turno)
                .ThenInclude(t => t!.Especialidad)
            .OrderByDescending(a => a.FechaHoraAcreditacion)
            .ToListAsync();
    }
    public async Task<int> AddAsync(AtencionAggregate atencion)
    {
        if (atencion is null)
            throw new ArgumentNullException(nameof(atencion));

        await _context.Atenciones.AddAsync(atencion);
        return atencion.IdAtencion;
    }
    public Task UpdateAsync(AtencionAggregate atencion)
    {
        if (atencion is null)
            throw new ArgumentNullException(nameof(atencion));

        _context.Atenciones.Update(atencion);
        return Task.CompletedTask;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}


