using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Aggregates;

namespace SePrise.Infrastructure.Persistence;

/// <summary>
/// Contexto principal de Entity Framework Core para el sistema SePrise.
/// </summary>
public class SePriseDbContext : DbContext
{
    // DbSets y configuraciones se agregarán en Microtarea 2.1

    public SePriseDbContext(DbContextOptions<SePriseDbContext> options) : base(options)
    {
    }
}
