using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Data.Configurations;

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> builder)
    {
        builder.ToTable("Proyectos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Descripcion)
            .HasMaxLength(200);
        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.Property(x => x.PropietarioId)
            .IsRequired();
        builder.HasOne(x => x.Propietario)
            .WithMany()
            .HasForeignKey(x => x.PropietarioId);

        builder.HasMany(x => x.Tareas)
            .WithOne(x => x.Proyecto)
            .HasForeignKey(x => x.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Historiales)
            .WithOne(x => x.Proyecto)
            .HasForeignKey(x => x.ProyectoId);
    }
}