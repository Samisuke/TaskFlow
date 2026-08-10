using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Requests;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Services
{
    public class TareaService : ITareaService
    {
        // Inyección de repositorios
        private readonly ITareaRepository _repoTarea;
        private readonly ITareaEtiquetaService _tareaEtiquetaService;
        private readonly ITareaPermissionService _tareaPermissions;
        private readonly IHistorialService _historialService;
        private readonly TaskFlowDbContext _context;

        public TareaService(
            ITareaRepository repoTarea,
            ITareaEtiquetaService TareaEtiquetaService,
            ITareaPermissionService tareaPermissions,
            IHistorialService historialService,
            TaskFlowDbContext context
        )
        {
            _repoTarea = repoTarea;
            _tareaEtiquetaService = TareaEtiquetaService;
            _tareaPermissions = tareaPermissions;
            _historialService = historialService;
            _context = context;
        }

        // Métodos GET
        // Obtener tus propias tareas pendientes. Útil para ver desde tu perfil que tienes por hacer.
        public async Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int usuarioId)
        {
            var tareas = await _repoTarea.ObtenerTareasPendientesPorUsuarioIdAsync(usuarioId);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas pendientes.");

            return Result <IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener tus tareas dadas. Útil para ver desde tu perfil el estado de las tareas que has mandado.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int usuarioId)
        {
            var tareas = await _repoTarea.ObtenerTareasDadasPorUsuarioIdAsync(usuarioId);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener todas las tareas de un proyecto. Útil para ver el estado del proyecto de un solo vistazo.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int proyectoId)
        {
            var tareas = await _repoTarea.ObtenerTareasDeUnProyectoAsync(proyectoId);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener una sola tarea. Útil para ver mas detalladamente y con más información una tarea concreta.
        public async Task <Result<Tarea?>> GetTareaPorIdAsync(int tareaId)
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(tareaId);
            if (tarea is null) return Result<Tarea?>.Mal("No se ha encontrado una tarea.");

            return Result<Tarea?>.Bien(tarea);
        }

        // Métodos POST
        // Crear una tarea
        public async Task <Result<Tarea>> PostTareaAsync(
            int propiaId,
            string tituloTarea,
            string descripcionTarea,
            EstadoTarea estadoTareaTarea,
            PrioridadTarea prioridadTareaTarea,
            DateTimeOffset fechaLimiteTarea,
            int proyectoId,
            int asignadoId,
            List<NuevaEtiqueta> etiquetas
        )
        {
            // Comprobaciones.
            if (!await _tareaPermissions.PuedePublicarTareasAsync(proyectoId, propiaId)) return Result<Tarea>.Mal("No se ha encontrado una tarea.");

            // Creación de transacción.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Creación de tarea.
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
                    CreadorId = propiaId
                };
                await _repoTarea.CrearTareaAsync(tarea);

                // Procesamos las etiquetas.
                if (etiquetas.Count > 0)
                {
                    var comprobarEtiquetas = await _tareaEtiquetaService.ComprobarSiEtiquetaExisteOCrearASync(etiquetas);
                    if (!comprobarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(comprobarEtiquetas.Error);

                    var asignarEtiquetas =  await _tareaEtiquetaService.AsignarEtiquetaATareaASync(tarea, etiquetas);
                    if (!asignarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(asignarEtiquetas.Error);
                }
                await _historialService.RegistrarTareaAsync(tarea);

                // Base de datos.
                await _context.SaveChangesAsync();

                // Commit de la transacción.
                await transaction.CommitAsync();

                return Result<Tarea>.Bien(tarea);
            }

            catch
            {   
                await transaction.RollbackAsync();
                throw;
            }   
        }

        // Métodos PATCH
        // Modificar una tarea (Sin contar su estado).
        public async Task <Result<Tarea>> PatchTareaAsync(
            int propiaId,
            int idTarea,
            string? tituloTarea,
            string? descripcionTarea,
            PrioridadTarea? prioridadTareaTarea,
            DateTimeOffset? fechaLimiteTarea,
            List<NuevaEtiqueta> etiquetas
        )
        {
            int numeroCambios = 0;

            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(idTarea);
            if (tarea is null) return Result<Tarea>.Mal("No se encuentra la tarea que quieres modificar.");

            // Comprobaciones.
            if (!await _tareaPermissions.PuedeModificarTareasAsync(propiaId, tarea)) return Result<Tarea>.Mal("No puedes modificar la tarea.");

            // Creación de la transacción
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
               // Realizar cambios.
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

                // Procesamos las etiquetas.
                if (etiquetas.Any())
                {
                    var comprobarEtiquetas = await _tareaEtiquetaService.ComprobarSiEtiquetaExisteOCrearASync(etiquetas);
                    if (!comprobarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(comprobarEtiquetas.Error);

                    var asignarEtiquetas =  await _tareaEtiquetaService.AsignarEtiquetaATareaASync(tarea, etiquetas);
                    if (!asignarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(asignarEtiquetas.Error);

                    if (asignarEtiquetas.EsCorrecto && comprobarEtiquetas.EsCorrecto) numeroCambios += 1;
                }
                // Base de datos.
                if (numeroCambios == 0) return Result<Tarea>.Mal("No se han detectado cambios.");
                await _historialService.ModificarTareaAsync(tarea, propiaId); 
                await _context.SaveChangesAsync();

                // Commit de la transacción.
                await transaction.CommitAsync();

                return Result<Tarea>.Bien(tarea);
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Modificar el estado de una tarea.
        // Separado del PATCH porque requiere comprobaciones diferentes por modelo de negocio..
        public async Task <Result<Tarea>> PatchEstadoTareaAsync(
            int propiaId, 
            int tareaId,
            EstadoTarea estadoTareaTarea
        )
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(tareaId);
            if (tarea is null) return Result<Tarea>.Mal("No se encuentra la tarea que quieres modificar.");

            // Comrpobaciones.
            if (!await _tareaPermissions.PuedeModificarEstadoTareaAsync(propiaId, tarea)) return Result<Tarea>.Mal("No puedes modificar el estado de una tarea.");
            
            // Transación
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Realizamos cambios
                tarea.Estado = estadoTareaTarea;    
               
                // Base de datos
                await _historialService.ModificarEstadoTareaAsync(tarea, propiaId);
                await _context.SaveChangesAsync();
                
                // Commit
                await transaction.CommitAsync();

                return Result<Tarea>.Bien(tarea);          
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}