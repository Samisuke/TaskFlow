using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

// Configuración de Entity Framework Core para la relación entre Tarea y Etiqueta.
// Define las claves, restricciones y relaciones de la tabla intermedia.

namespace TaskFlow.Infrastructure.Data.Configurations;

public class TareaEtiquetaConfiguration : IEntityTypeConfiguration<TareaEtiqueta>
{
    public void Configure(EntityTypeBuilder<TareaEtiqueta> builder)
    {
        builder.ToTable("TareaEtiqueta");
        builder.HasKey(x => new
        {
            x.TareaId,
            x.EtiquetaId
        });
        
        builder.HasOne(x => x.Tarea)
            .WithMany(x => x.Etiquetas)
            .HasForeignKey(x => x.TareaId);

        builder.HasOne(x => x.Etiqueta)
            .WithMany(x => x.Tareas)
            .HasForeignKey(x => x.EtiquetaId);
    }
}