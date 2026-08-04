using Taskflow.Core.Repositories;
using Taskflow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Requests;


namespace TaskFlow.Infrastructure.Services
{
    public class TareaService : ITareaService
    {
        // Inyección de repositorios
        private readonly ITareaRepository _repoTarea;
        private readonly ITareaEtiquetaService _TareaEtiquetaService;
        private readonly ITareaPermissionService _tareaPermissions;
        private readonly IHIstorialService _historialService;

        public TareaService(ITareaRepository repoTarea,
        ITareaEtiquetaService TareaEtiquetaService,
        ITareaPermissionService tareaPermissions,
        IHIstorialService historialService
        )
        {
            _repoTarea = repoTarea;
            _TareaEtiquetaService = TareaEtiquetaService;
            _tareaPermissions = tareaPermissions;
            _historialService = historialService;
        }

        // Métodos GET
        // Obtener tus propias tareas pendientes. Útil para ver desde tu perfil que tienes por hacer.
        public async Task <Result<IEnumerable<Tarea>>> GetTareasPendientesDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasPendientesPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas pendientes.");

            return Result <IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener tus tareas dadas. Útil para ver desde tu perfil el estado de las tareas que has mandado.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDadasDeUsuarioAsync(int idUsuario)
        {
            var tareas = await _repoTarea.ObtenerTareasDadasPorUsuarioIdAsync(idUsuario);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener todas las tareas de un proyecto. Útil para ver el estado del proyecto de un solo vistazo.
        public async Task<Result<IEnumerable<Tarea>>> GetTareasDeUnProyectoAsync(int idProyecto)
        {
            var tareas = await _repoTarea.ObtenerTareasDeUnProyectoAsync(idProyecto);
            if (!tareas.Any()) return Result<IEnumerable<Tarea>>.Mal("No se encuentran tareas dadas por este usuario.");

            return Result<IEnumerable<Tarea>>.Bien(tareas);
        }

        // Obtener una sola tarea. Útil para ver mas detalladamente y con más información una tarea concreta.
        public async Task <Result<Tarea?>> GetTareaPorIdAsync(int id)
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(id);
            if (tarea is null) return Result<Tarea?>.Mal("No se ha encontrado una tarea.");

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
            int asignadoId,
            IEnumerable<NuevaEtiqueta> etiquetas
        )
        {
            // Comprobaciones.
            if (!await _tareaPermissions.PuedePublicarTareasAsync(proyectoId, idPropia)) return Result<Tarea>.Mal("No se ha encontrado una tarea.");

            // Creacion de tarea.
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

            // Base de datos.
            await _repoTarea.CrearTareaAsync(tarea);
            var guardadoExistoso = await _repoTarea.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<Tarea>.Mal("Fallo inesperado al guardar la tarea. Inténtalo de nuevo más tarde.");

            // Procesamos las etiquetas
            if (etiquetas.Any())
            {
                var comprobarEtiquetas = await _TareaEtiquetaService.ComprobarSiEtiquetaExisteOCrearASync(etiquetas);
                if (!comprobarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(comprobarEtiquetas.Error);

                var asignarEtiquetas =  await _TareaEtiquetaService.AsignarEtiquetaATareaASync(tarea, etiquetas);
                if (!asignarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(asignarEtiquetas.Error);
            }

            await _historialService.RegistrarTareaAsync(tarea);

            return Result<Tarea>.Bien(tarea);
        }

        // Métodos PATCH
        // Modificar una tarea (Sin contar su estado).
        public async Task <Result<Tarea>> PatchTareaAsync(
            int idPropia,
            int idTarea,
            string? tituloTarea,
            string? descripcionTarea,
            PrioridadTarea? prioridadTareaTarea,
            DateTimeOffset? fechaLimiteTarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        )
        {
            int numeroCambios = 0;

            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(idTarea);
            if (tarea is null) return Result<Tarea>.Mal("No se encuentra la tarea que quieres modificar.");

            // Comprobaciones.
            if (!await _tareaPermissions.PuedeModificarTareasAsync(idPropia, tarea)) return Result<Tarea>.Mal("No puedes modificar la tarea.");

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

            // Procesamos las etiquetas
            if (etiquetas.Any())
            {
                var comprobarEtiquetas = await _TareaEtiquetaService.ComprobarSiEtiquetaExisteOCrearASync(etiquetas);
                if (!comprobarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(comprobarEtiquetas.Error);

                var asignarEtiquetas =  await _TareaEtiquetaService.AsignarEtiquetaATareaASync(tarea, etiquetas);
                if (!asignarEtiquetas.EsCorrecto) return Result<Tarea>.Mal(asignarEtiquetas.Error);

                numeroCambios += 1;
            }
            
            // Base de datos.
            if (numeroCambios == 0) return Result<Tarea>.Mal("No se han detectado cambios.");
            var guardadoExitoso = await _repoTarea.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Tarea>.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");
            
            await _historialService.ModificarTareaAsync(tarea, idPropia);

            return Result<Tarea>.Bien(tarea);
        }

        // Modificar el estado de una tarea. Separado del PATCH normal por modelo de negocio.
        public async Task <Result<Tarea>> PatchEstadoTareaAsync(
            int idPropia, 
            int idTarea,
            EstadoTarea estadoTareaTarea
        )
        {
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(idTarea);
            if (tarea is null) return Result<Tarea>.Mal("No se encuentra la tarea que quieres modificar.");

            // Comrpobaciones.
            if (!await _tareaPermissions.PuedeModificarEstadoTareaAsync(idPropia, tarea)) return Result<Tarea>.Mal("No puedes modificar el estado de una tarea.");

            // Realizamos cambios
            tarea.Estado = estadoTareaTarea;

            // Base de datos
            var guardadoExitoso = await _repoTarea.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Tarea>.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            await _historialService.ModificarEstadoTareaAsync(tarea, idPropia);
            
            return Result<Tarea>.Bien(tarea);

        }
    }
}