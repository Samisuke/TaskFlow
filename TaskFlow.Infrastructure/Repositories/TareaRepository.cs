using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TareaRepository : ITareaRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        public TareaRepository(TaskFlowDbContext context)
        {
            _context = context;
        }
         
        // Métodos GET
        // Todas las tareas pendientes con proyecto y creador de un usuario.
        public async Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasPendientesPorUsuarioIdAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        )
        {
            var query = _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Asignado)
                .Include(t => t.Creador)
                .Include(t => t.Etiquetas)
                    .ThenInclude(te => te.Etiqueta)
                .Where(t => t.AsignadoId == usuarioId);
            
            var totalItems = await query.CountAsync();
            var tareas = await query
                .OrderBy(t => t.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return (tareas, totalItems);
        }

        // Todas las tareas dadas con proyecto y asignado de un usuario.
        public async Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasDadasPorUsuarioIdAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        )
        {
            var query = _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Asignado)
                .Include(t => t.Creador)
                .Include(t => t.Etiquetas)
                    .ThenInclude(te => te.Etiqueta)
                .Where(t => t.CreadorId == usuarioId);

            var totalItems = await query.CountAsync();
            var tareas = await query
                .OrderBy(t => t.Id)
                .Skip((pagina -1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return (tareas, totalItems);
        }

        // Todas las tareas de un proyecto con creador y asignado.
        public async Task <(IEnumerable<Tarea> Tareas, int TotalITems)> ObtenerTareasDeUnProyectoAsync(
            int proyectoId,
            int pagina,
            int tamanoPagina
        )
        {
            var query = _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Creador)
                .Include(t => t.Asignado)
                .Include(t => t.Etiquetas)
                    .ThenInclude(te => te.Etiqueta)
                .Where(t => t.ProyectoId == proyectoId);      
                
            var totalItems = await query.CountAsync();
            var tareas = await query
                .OrderBy(t => t.Id)
                .Skip((pagina -1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return (tareas, totalItems); 
        }

        // Una tarea completa.
        public async Task <Tarea?> ObtenerTareaPorIdAsync(int idTarea)
        {
            return await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Creador)
                .Include(t => t.Asignado)
                .Include(t => t.Etiquetas)
                    .ThenInclude(te => te.Etiqueta)
                .FirstOrDefaultAsync(t => t.Id == idTarea);   
        }

        // Métodos POST
        public async Task CrearTareaAsync(Tarea tarea)
        {
            await _context.AddAsync(tarea);
        }
    }
}