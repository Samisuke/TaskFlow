using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Taskflow.Core.Repositories;

namespace TaskFlow.Infrastructure.Services
{
    public class HistorialService : IHIstorialService
    {
        // Inyección del repositorio
        private readonly IHistorialRepository _repoHistorial;

        public HistorialService(IHistorialRepository repoHistorial)
        {
            _repoHistorial = repoHistorial;
        }

        // Peticiones GET
        // Obtener el historial. Útil para mostrar el historial de cambios que ha sufrido una tarea.
        public async Task<Result<IEnumerable<Historial>>> GetHistorialDeUnaTareaAsync(int idTarea)
        {
            var historial = await _repoHistorial.ObtenerHistorialDeUnaTareaAsync(idTarea);
            if (!historial.Any()) return Result<IEnumerable<Historial>>.Mal("La tarea no tiene cambios realizados.");

            return Result<IEnumerable<Historial>>.Bien(historial);
        }

        //Peticiones POST
        // Crear un "cambio" en la tarea.
        public async Task<Result<Historial>> PostTareaAsync(
            int tareaId,
            int usuarioId,
            string accion
        )
        {
            var historial = new Historial
            {
                TareaId = tareaId,
                Accion = accion,
                UsuarioId = usuarioId,
                Fecha = DateTime.UtcNow
            };

            var guardadoExitoso = await _repoHistorial.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Historial>.Mal("ERROR. Fallo inesperado al guardar el historial. Inténtalo de nuevo más tarde.");

            return Result<Historial>.Bien(historial);
        }
    }
}