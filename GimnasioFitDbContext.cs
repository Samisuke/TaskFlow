using Microsoft.EntityFrameworkCore;
using GimnasioFit.Core.Models;

namespace GimnasioFit.Infrastructure.Data
{
    public class GimnasioFitDbContext : DbContext
    {
        public GimnasioFitDbContext(DbContextOptions options) : base(options)
        {
            
        }
        
        public DbSet<Socio> ListaSocios {get; set;}
        public DbSet<Empleado> ListaEmpleados {get; set;}
        public DbSet<Clase> ListaClases {get; set;}
        public DbSet<Reserva> ListaReservas {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}