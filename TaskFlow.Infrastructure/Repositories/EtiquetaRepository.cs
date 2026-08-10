using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class EtiquetaRepository : IEtiquetaRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;

        public EtiquetaRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Métodos GET
        // Una etiqueta simple por ID
        public async Task <Etiqueta?> ObtenerUnaEtiquetaPorIdAsync(int idEtiqueta)
        {
            return await _context.Etiquetas
            .FirstOrDefaultAsync(e => e.Id == idEtiqueta);   
        }

        // Una etiqueta simple por nombre y color.
        public async Task <Etiqueta?> ObtenerEtiquetaPorNombreYColorAsync(string nombre, string color)
        {
            return await _context.Etiquetas
                .FirstOrDefaultAsync(e => e.Nombre == nombre
                && e.Color == color);
        }

        // Métodos POST
        public async Task CrearEtiquetaAsync(Etiqueta etiqueta)
        {
            await _context.Etiquetas.AddAsync(etiqueta);
        }
    }
}