using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Aggregates;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para el agregado AtencionAggregate.
/// </summary>
public class AtencionConfiguration : IEntityTypeConfiguration<AtencionAggregate>
{
    public void Configure(EntityTypeBuilder<AtencionAggregate> builder)
    {
        // Configuraciones de tabla y columnas se agregarán en Microtarea 2.2
    }
}
