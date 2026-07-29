using Taskflow.Core.Repositories;
using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;


namespace Task.Infrastructure.Service
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
        public async Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasPendientesPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas pendientes.");

            return Result <IEnumerable<Tarea>>.Bien(tareas);
        }

        public async Task<Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasDadasPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        public async Task<Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int idProyecto)
        {
            var tareas = await _repoTarea.ObtenerTareasDeUnProyectoAsync(idProyecto);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("ERROR. No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        public async Task <Result<Tarea?>> GetTareaPorIdAsync(int id)
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(id);
            if (tarea is null) return Result<Tarea?>.Mal("ERROR. No se ha encontrado una tarea.");

            return Result<Tarea?>.Bien(tarea);
        }

        // Métodos POST
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