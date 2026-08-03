using Taskflow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Requests;

namespace Taskflow.Infrastructure.Services
{
    public class TareaEtiquetaService : ITareaEtiquetaService
    {
        // Inyección del repositorio
        private readonly ITareaEtiquetaRepository _repoTareaEtiqueta;
        private readonly IEtiquetaRepository _repoEtiqueta;

        public TareaEtiquetaService(
            ITareaEtiquetaRepository repoTareaEtiqueta,
            IEtiquetaRepository repoEtiqueta
        )
        {
            _repoTareaEtiqueta = repoTareaEtiqueta;
            _repoEtiqueta = repoEtiqueta;
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
        public async Task<Result> PostTareaEtiquetaAsync(
            Tarea tarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        )
        {
            // Comprobamos cada elemento de la lista
            foreach (var NuevaEtiqueta in etiquetas)
            {
                // Comprobación: La etiqueta tiene que existir.
                var etiqueta = await _repoEtiqueta.ObtenerEtiquetaPorNombreYColorAsync(NuevaEtiqueta.Nombre, NuevaEtiqueta.Color);
                
                // Si no existe, la creamos.
                if (etiqueta is null)
                {     
                    etiqueta = new Etiqueta
                    {
                        Nombre = NuevaEtiqueta.Nombre,
                        Color = NuevaEtiqueta.Color
                    };
                    
                    // La guardamos.
                    await _repoEtiqueta.CrearEtiquetaAsync(etiqueta);
                    var guardadoExitosoEtiqueta = await _repoEtiqueta.GuardarCambiosAsync();
                    if (!guardadoExitosoEtiqueta) return Result.Mal("Fallo inesperado al guardar la etiqueta. Inténtalo de nuevo más tarde.");
                }

                // Comprobación: Que la tarea no tenga ya esta misma etiqueta.
                bool relacionExiste = await _repoTareaEtiqueta.ExisteRelacionAsync(tarea.Id, etiqueta.Id);
                if (relacionExiste) continue;
                
                // Creamos la relación
                await _repoTareaEtiqueta.CrearTareaEtiquetaAsync(new TareaEtiqueta
                {
                    TareaId = tarea.Id,
                    EtiquetaId = etiqueta.Id
                });
            }

            await _repoTareaEtiqueta.GuardarCambiosAsync();
            return Result.Bien();
        }
    }
}