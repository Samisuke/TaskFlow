using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Requests;

namespace TaskFlow.Infrastructure.Services
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
        public async Task<Result<IEnumerable<TareaEtiqueta>>> GetEtiquetasDeUnaTareaAsync(int tareaId)
        {
            var etiquetas = await _repoTareaEtiqueta.ObtenerEtiquetasDeUnaTareaAsync(tareaId);
            if (!etiquetas.Any()) return Result<IEnumerable<TareaEtiqueta>>.Mal("La tarea no tiene etiquetas.");

            return Result<IEnumerable<TareaEtiqueta>>.Bien(etiquetas);
        }

        // Misc.
        // Comprobamos si una etiqueta ya existe con un nombre y color que recibimos de la tarea. Si existe, devolvemos esa etiqueta.
        // Si no existe, la creamos.
        public async Task<Result> ComprobarSiEtiquetaExisteOCrearASync(IEnumerable<NuevaEtiqueta> etiquetas)
        {
            // Comprobamos cada elemento de la lista
            foreach (var nuevaEtiqueta in etiquetas)
            {
                // Comprobación: La etiqueta tiene que existir.
                var etiqueta = await _repoEtiqueta.ObtenerEtiquetaPorNombreYColorAsync(nuevaEtiqueta.Nombre, nuevaEtiqueta.Color);
                
                // Si no existe, la creamos.
                if (etiqueta is null)
                {     
                    etiqueta = new Etiqueta
                    {
                        Nombre = nuevaEtiqueta.Nombre,
                        Color = nuevaEtiqueta.Color
                    };

                    // La metemos en la base de datos.
                    await _repoEtiqueta.CrearEtiquetaAsync(etiqueta);
                }
            }

            // Guardamos los cambios
            var guardadoExitosoEtiqueta = await _repoEtiqueta.GuardarCambiosAsync();
            if (!guardadoExitosoEtiqueta) return Result.Mal("Fallo inesperado al guardar la etiqueta. Inténtalo de nuevo más tarde.");

            return Result.Bien();   
        }

        // Asignar etiquetas a una tarea concreta. Separado de la comprobación para mantener
        // las responsabilidades separadas y construir métodos escalables.
        public async Task<Result> AsignarEtiquetaATareaASync(
            Tarea tarea,
            IEnumerable<NuevaEtiqueta> etiquetas
        )
        {
            foreach (var etiqueta in etiquetas)
            {
                var etiquetaNueva = await _repoEtiqueta.ObtenerEtiquetaPorNombreYColorAsync(etiqueta.Nombre, etiqueta.Color);
                if (etiquetaNueva is null) return Result.Mal("Error interno.");

                // Comprobación: Que la tarea no tenga ya esta misma etiqueta.
                bool relacionExiste = await _repoTareaEtiqueta.ExisteRelacionAsync(tarea.Id, etiquetaNueva.Id);
                if (relacionExiste) continue;

                // Creamos la relación
                await _repoTareaEtiqueta.CrearTareaEtiquetaAsync(new TareaEtiqueta
                {
                    TareaId = tarea.Id,
                    EtiquetaId = etiquetaNueva.Id
                });          
            }

            var guardadoExitoso = await _repoTareaEtiqueta.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result.Bien();       
        }
    }
}