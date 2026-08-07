using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

// Configuración de Entity Framework Core para la entidad Etiqueta.
// Define las restricciones, relaciones y propiedades de la tabla Etiqueta.

namespace TaskFlow.Infrastructure.Data.Configurations;

public class EtiquetaConfiguration : IEntityTypeConfiguration<Etiqueta>
{
    public void Configure(EntityTypeBuilder<Etiqueta> builder)
    {
        builder.ToTable("Etiquetas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(x => x.Color)
            .IsRequired();
    }
}