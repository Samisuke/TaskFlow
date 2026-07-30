using Taskflow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;


namespace TaskFlow.Infrastructure.Services
{
    public class TareaService : ITareaService
    {
        // Inyección de repositorios
        private readonly ITareaRepository _repoTarea;

        public TareaService(ITareaRepository repoTarea)
        {
            _repoTarea = repoTarea;
        }

        // Métodos GET
        // Obtener tus propias tareas pendientes. Útil para ver desde tu perfil que tienes por hacer.
        public async Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasPendientesPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas pendientes.");

            return Result <IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener tus tareas dadas. Útil para ver desde tu perfil el estado de las tareas que has mandado.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasDadasPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener todas las tareas de un proyecto. Útil para ver el estado del proyecto de un solo vistazo.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int idProyecto)
        {
            var tareas = await _repoTarea.ObtenerTareasDeUnProyectoAsync(idProyecto);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener una sola tarea. Útil para ver mas detalladamente y con más información una tarea concreta.
        public async Task <Result<Tarea?>> GetTareaPorIdAsync(int id)
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(id);
            if (tarea is null) return Result<Tarea?>.Mal("ERROR. No se ha encontrado una tarea.");

            return Result<Tarea?>.Bien(tarea);
        }

        // Métodos POST
        // Crear una tarea
        public async Task <Result<Tarea>> PostTareaAsync(
            int idPropia,
            string tituloTarea,
            string descripcionTarea,
            EstadoTarea estadoTareaTarea,
            PrioridadTarea prioridadTareaTarea,
            DateTimeOffset fechaLimiteTarea,
            int proyectoId,
            int asignadoId
        )
        {
            var tarea = new Tarea
            {
                Titulo = tituloTarea,
                Descripcion = descripcionTarea,
                Estado = estadoTareaTarea,
                Prioridad = prioridadTareaTarea,
                FechaCreacion = DateTime.UtcNow,
                FechaLimite = fechaLimiteTarea,
                ProyectoId = proyectoId,
                AsignadoId = asignadoId,
                CreadorId = idPropia
            };

            await _repoTarea.CrearTareaAsync(tarea);
            var guardadoExistoso = await _repoTarea.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<Tarea>.Mal("ERROR. Fallo inesperado al guardar la tarea. Inténtalo de nuevo más tarde.");

            return Result<Tarea>.Bien(tarea);
        }

        // Métodos PATCH
        // Modificar una tarea.
        public async Task <Result<Tarea>> PatchTareaAsync(
            int id,
            string? tituloTarea,
            string? descripcionTarea,
            EstadoTarea? estadoTareaTarea,
            PrioridadTarea? prioridadTareaTarea,
            DateTimeOffset? fechaLimiteTarea
        )
        {
            int numeroCambios = 0;

            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(id);
            if (tarea is null) return Result<Tarea>.Mal("ERROR. No se ha encontrado una tarea.");

            if (tituloTarea is not null)
            {
                tarea.Titulo = tituloTarea;
                numeroCambios += 1;
            }
            if (descripcionTarea is not null)
            {
                tarea.Descripcion = descripcionTarea;
                numeroCambios += 1;
            }
            if (estadoTareaTarea.HasValue)
            {
                tarea.Estado = estadoTareaTarea.Value;
                numeroCambios += 1; 
            }
            if (prioridadTareaTarea.HasValue)
            {
                tarea.Prioridad = prioridadTareaTarea.Value;
                numeroCambios += 1; 
            }
            if (fechaLimiteTarea.HasValue)
            {
                tarea.FechaLimite = fechaLimiteTarea.Value;
                numeroCambios += 1; 
            }
            if (numeroCambios == 0) return Result<Tarea>.Mal("ERROR. No se han detectado cambios.");
            var guardadoExitoso = await _repoTarea.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Tarea>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");
            
            return Result<Tarea>.Bien(tarea);
        }
    }
}