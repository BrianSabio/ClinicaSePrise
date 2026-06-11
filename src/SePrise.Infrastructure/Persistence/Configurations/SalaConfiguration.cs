using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Sala.
/// </summary>
public class SalaConfiguration : IEntityTypeConfiguration<Sala>
{
    public void Configure(EntityTypeBuilder<Sala> builder)
    {
        // Configuraciones de tabla y columnas se agregarán en Microtarea 2.2
    }
}
