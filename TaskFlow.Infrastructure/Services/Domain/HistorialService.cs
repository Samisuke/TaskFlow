using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Requests;

namespace TaskFlow.Infrastructure.Services
{
    public class HistorialService : IHistorialService
    {
        // Inyección del repositorio
        private readonly IHistorialRepository _repoHistorial;

        public HistorialService(
            IHistorialRepository repoHistorial 
        )
        {
            _repoHistorial = repoHistorial;
        }

        // Peticiones GET
        // Obtener el historial. Útil para mostrar el historial de cambios que ha sufrido un proyecto.
        public async Task<Result<IEnumerable<Historial>>> GetHistorialAsync(int proyectoId)
        {
            var historial = await _repoHistorial.ObtenerHistorialAsync(proyectoId);
            if (!historial.Any()) return Result<IEnumerable<Historial>>.Mal("La tarea no tiene cambios realizados.");

            return Result<IEnumerable<Historial>>.Bien(historial);
        }
        
        // Método privado "padre"
        private async Task RegistrarAsync(
            int proyectoId,
            int usuarioId,
            string accion)
        {
            var historial = new Historial
            {
                ProyectoId = proyectoId,
                UsuarioId = usuarioId,
                Accion = accion,
                Fecha = DateTime.UtcNow
            };

            await _repoHistorial.CrearHistorialAsync(historial);
            // No guarda cambios. El servicio que lo llame guardará los cambios.
        }

        // Registrar un comentario
        public async Task RegistrarComentarioAsync(Comentario comentario, int propiaId)
        {
            await RegistrarAsync(
                comentario.Tarea!.ProyectoId,
                propiaId,
                HistorialActions.ComentarioCreado);
        }

        // Modificar un comentario
        public async Task ModificarComentarioAsync(Comentario comentario, int propiaId)
        {
            await RegistrarAsync(
                comentario.Tarea!.ProyectoId,
                propiaId,
                HistorialActions.ComentarioModificado);
        }

        // Modificar un proyecto
        public async Task ModificarProyectoAsync(Proyecto proyecto, int propiaId)
        {
            await RegistrarAsync(
                proyecto.Id,
                propiaId,
                HistorialActions.ProyectoModificado);
        }

        // Modificar dueño de un proyecto
        public async Task ModificarDueñoProyectoAsync(Proyecto proyecto, int propiaId)
        {
            await RegistrarAsync(
                proyecto.Id,
                propiaId,
                HistorialActions.ProyectoDueñoModificado);
        }

        // Añadir persona a un proyecto
        public async Task AñadirPersonaProyectoAsync(int proyectoId, int propiaId)
        {
            await RegistrarAsync(
                proyectoId,
                propiaId,
                HistorialActions.AñadirPersona);
        }

        // Modificar persona de un proyecto
        public async Task ModificarPersonaProyectoAsync(int proyectoId, int propiaId)
        {
            await RegistrarAsync(
                proyectoId,
                propiaId,
                HistorialActions.ModificarPersonaEnProyecto);
        }

        // Registrar una tarea
        public async Task RegistrarTareaAsync(Tarea tarea)
         {
            await RegistrarAsync(
                tarea.ProyectoId,
                tarea.CreadorId,
                HistorialActions.AñadirTarea);
        }

        // Modificar una tarea
        public async Task ModificarTareaAsync(Tarea tarea, int propiaId)
         {
            await RegistrarAsync(
                tarea.ProyectoId,
                propiaId,
                HistorialActions.ModificarTarea);
        }

        // Modificar el estado de una  tarea
        public async Task ModificarEstadoTareaAsync(Tarea tarea, int propiaId)
         {
            await RegistrarAsync(
                tarea.ProyectoId,
                propiaId,
                HistorialActions.ModificarEstadoTarea);
        }
    }
}