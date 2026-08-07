using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

// Configuración de Entity Framework Core para la entidad Comentario.
// Define las restricciones, relaciones y propiedades de la tabla Comentario.

namespace TaskFlow.Infrastructure.Data.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Contenido)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Fecha)
            .IsRequired();
            
        builder.Property(x => x.UsuarioId)
            .IsRequired();
        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.UsuarioId);

        builder.Property(x => x.TareaId)
            .IsRequired();
        builder.HasOne(x => x.Tarea)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.TareaId);
    }
}