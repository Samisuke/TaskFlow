using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Data.Configurations;

public class HistorialConfiguration : IEntityTypeConfiguration<Historial>
{
    public void Configure(EntityTypeBuilder<Historial> builder)
    {
        builder.ToTable("Historiales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Accion)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.TareaId)
            .IsRequired();
        builder.HasOne(x => x.Tarea)
            .WithMany(x => x.Historiales)
            .HasForeignKey(x => x.UsuarioId);


        builder.Property(x => x.UsuarioId)
            .IsRequired();
        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.TareaId);
    }
}