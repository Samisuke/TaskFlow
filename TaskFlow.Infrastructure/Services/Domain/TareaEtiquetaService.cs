using Taskflow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;

namespace Taskflow.Infrastructure.Services
{
    public class TareaEtiquetaService : ITareaEtiquetaService
    {
        // Inyección del repositorio
        private readonly ITareaEtiquetaRepository _repoTareaEtiqueta;
        private readonly IEtiquetaRepository _repoEtiqueta;
        private readonly ITareaRepository _repoTarea;
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;

        public TareaEtiquetaService(
            ITareaEtiquetaRepository repoTareaEtiqueta,
            IEtiquetaRepository repoEtiqueta,
            ITareaRepository repoTarea,
            IProyectoUsuarioRepository repoUsuario
        )
        {
            _repoTareaEtiqueta = repoTareaEtiqueta;
            _repoEtiqueta = repoEtiqueta;
            _repoTarea = repoTarea;
            _repoProyectoUsuario = repoUsuario;
        }
        
        // Métodos GET
        // Obtener todas las etiquetas de una tarea. Útil para mostrarlas junto a las tareas.
        public async Task<Result<IEnumerable<TareaEtiqueta>>> GetEtiquetasDeUnaTareaAsync(int idTarea)
        {
            var etiquetas = await _repoTareaEtiqueta.ObtenerEtiquetasDeUnaTareaAsync(idTarea);
            if (!etiquetas.Any()) return Result<IEnumerable<TareaEtiqueta>>.Mal("La tarea no tiene etiquetas.");

            return Result<IEnumerable<TareaEtiqueta>>.Bien(etiquetas);
        }

        // Método POST
        // Vincular una etiqueta a una tarea.
        public async Task<Result<TareaEtiqueta>> PostTareaEtiquetaAsync(
            int idPropia,
            int tareaId,
            string nombreEtiqueta,
            string colorEtiqueta
        )
        {
            // Comprobación: La tarea tiene que existir.
            var tarea = await _repoTarea.ObtenerTareaPorIdAsync(tareaId);
            if (tarea is null) return Result<TareaEtiqueta>.Mal("La tarea no existe.");

            // Comprobación: Tienes que pertener al proyecto para poner etiquetas.
            var proyectoUsuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(tarea.ProyectoId, idPropia);
            if (proyectoUsuario is null) return Result<TareaEtiqueta>.Mal("No puedes poner etiquetas en un proyecto al que no perteneces.");

            // Comprobación: La etiqueta tiene que existir.
            var etiqueta = await _repoEtiqueta.ObtenerEtiquetaPorNombreYColorAsync(nombreEtiqueta, colorEtiqueta);
            if (etiqueta is null)
            {
                // Si no existe, la creamos.
                etiqueta = new Etiqueta
                {
                    Nombre = nombreEtiqueta,
                    Color = colorEtiqueta
                };

                // La guardamos.
                await _repoEtiqueta.CrearEtiquetaAsync(etiqueta);
                var guardadoExitosoEtiqueta = await _repoEtiqueta.GuardarCambiosAsync();
                if (!guardadoExitosoEtiqueta) return Result<TareaEtiqueta>.Mal("ERROR. Fallo inesperado al guardar la etiqueta. Inténtalo de nuevo más tarde.");
            }
            
            // Comprobación: Que la tarea no tenga ya esta misma etiqueta.
            bool relacionExiste = await _repoTareaEtiqueta.ExisteRelacionAsync(tareaId, etiqueta.Id);
            if (relacionExiste) return Result<TareaEtiqueta>.Mal("La tarea ya tiene esta etiqueta.");

            // Creación.
            var tareaEtiqueta = new TareaEtiqueta
            {
                TareaId = tareaId,
                EtiquetaId = etiqueta.Id
            };

            await _repoTareaEtiqueta.CrearTareaEtiquetaAsync(tareaEtiqueta);    
            var guardadoExitoso = await _repoTareaEtiqueta.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<TareaEtiqueta>.Mal("ERROR. Fallo inesperado al asignar la etiqueta. Inténtalo de nuevo más tarde.");

            return Result<TareaEtiqueta>.Bien(tareaEtiqueta);
        }
    }
}