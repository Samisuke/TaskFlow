using TaskFlow.Infrastructure.Data;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Repositories;

namespace TaskFlow.Infrastructure.Repositories
{
    public class HistorialRepository : IHistorialRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        
        public HistorialRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Métodos GET
        // Ver el historial completo.
        public async Task <IEnumerable<Historial>> ObtenerHistorialDeUnaTareaAsync(int idTarea)
        {
            return await _context.Historiales
                .Include(h => h.Usuario)
                .ToListAsync();
        }

        // Métodos POST
        // Crear el historial
        public async Task CrearHistorialAsync(Historial historial)
        {
            await _context.AddAsync(historial);
        }
        
        // Misc.
        // Guardar
        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}