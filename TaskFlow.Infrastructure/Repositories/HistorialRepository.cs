using TaskFlow.Infrastructure.Data;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Repositories
{
    public class HistorialRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        
        public HistorialRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Peticiones GET
        public async Task <IEnumerable<Historial>> ObtenerHistorialDeUnaTareaAsync(int idTarea)
        {
            return await _context.Historiales
                .Include(h => h.Usuario)
                .ToListAsync();
        }

        // Misc.
        public async Task CrearHistorialAsync(Historial historial)
        {
            await _context.AddAsync(historial);
        }
        
        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}