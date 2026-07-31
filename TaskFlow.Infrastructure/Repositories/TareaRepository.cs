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
        public async Task <IEnumerable<Tarea>> ObtenerTareasPendientesPorUsuarioIdAsync(int idUsuario)
        {
            return await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Creador)
                .Where(t => t.AsignadoId == idUsuario)
                .ToListAsync();
        }

        // Todas las tareas dadas con proyecto y asignado de un usuario.
        public async Task <IEnumerable<Tarea>> ObtenerTareasDadasPorUsuarioIdAsync(int idUsuario)
        {
              return await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Asignado)
                .Where(t => t.CreadorId == idUsuario)
                .ToListAsync();          
        }

        // Todas las tareas de un proyecto con creador y asignado.
        public async Task <IEnumerable<Tarea>> ObtenerTareasDeUnProyectoAsync(int idProyecto)
        {
            return await _context.Tareas
                .Include(t => t.Creador)
                .Include(t => t.Asignado)
                .Where(t => t.ProyectoId == idProyecto)
                .ToListAsync();         
        }

        // Una tarea completa.
        public async Task <Tarea?> ObtenerTareaPorIdAsync(int idTarea)
        {
            return await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.Creador)
                .Include(t => t.Asignado)
                .FirstOrDefaultAsync(t => t.Id == idTarea);   
        }

        // Métodos POST
        public async Task CrearTareaAsync(Tarea tarea)
        {
            await _context.AddAsync(tarea);
        }

        // Misc.
        // Guardar
        public async Task<bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}