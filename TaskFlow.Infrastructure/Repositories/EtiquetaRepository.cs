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
        // Obtención de Etiquetas
        // Todas las etiquetas simples
        public async Task <IEnumerable<Etiqueta>> ObtenerTodasLasEtiquetasSimplesAsync()
        {
            return await _context.Etiquetas
                .ToListAsync();
        }

        // Todas las etiquetas simples de una tarea concreta 
        public async Task <IEnumerable<Etiqueta>> ObtenerTodasLasEtiquetasDeUnaTareaAsync(int idTarea)
        {
            return await _context.Etiquetas
                .Where(e => e.Tareas
                    .Any(e => e.TareaId == idTarea))
                .ToListAsync();
        }

        // Una etiqueta completa por ID
        public async Task <Etiqueta?> ObtenerUnaEtiquetaPorIdAsync(int idEtiqueta)
        {
            return await _context.Etiquetas
            .FirstOrDefaultAsync(e => e.Id == idEtiqueta);   
        }

        // Misc.
        public async Task CrearEtiquetaAsync(Etiqueta etiqueta)
        {
            await _context.Etiquetas.AddAsync(etiqueta);
        }

        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}