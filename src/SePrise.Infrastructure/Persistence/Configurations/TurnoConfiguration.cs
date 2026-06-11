using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Aggregates;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para el agregado TurnoAggregate.
/// </summary>
public class TurnoConfiguration : IEntityTypeConfiguration<TurnoAggregate>
{
    public void Configure(EntityTypeBuilder<TurnoAggregate> builder)
    {
        // Configuraciones de tabla y columnas se agregarán en Microtarea 2.2
    }
}
