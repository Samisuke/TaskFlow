using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TareaEtiquetaRepository : ITareaEtiquetaRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        
        public TareaEtiquetaRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Obtención de etiquetas de una tarea.
        public async Task <IEnumerable<TareaEtiqueta>> ObtenerEtiquetasDeUnaTareaAsync(int idTarea)
        {
            return await _context.TareaEtiquetas
                .Where(x => x.TareaId == idTarea)
                .Include(x => x.Etiqueta)
                .Include(x => x.Tarea)
                .ToListAsync();
        }

        // Métodos GET
        // Asignar etiqueta a una tarea.
        public async Task CrearTareaEtiquetaAsync(TareaEtiqueta tareaEtiqueta)
        {
            await _context.TareaEtiquetas.AddAsync(tareaEtiqueta);
        }

        // Misc.
        // Comprobar una relacion.
        public async Task <bool> ExisteRelacionAsync(int tareaId, int etiquetaId)
        {
            return await _context.TareaEtiquetas.AnyAsync(x =>
            x.TareaId == tareaId &&
            x.EtiquetaId == etiquetaId);
        }
    }
}