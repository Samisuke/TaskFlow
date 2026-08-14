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
        public async Task<Result<List<Etiqueta>>> ComprobarSiEtiquetaExisteOCrearASync(IEnumerable<NuevaEtiqueta> etiquetas)
        {
            List<Etiqueta> listaEtiquetas = [];
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

                listaEtiquetas.Add(etiqueta);
            }

            // Return sin guardar cambios porque el servicio lo guarda todo mediante la transacción
            return Result<List<Etiqueta>>.Bien(listaEtiquetas);   
        }

        // Asignar etiquetas a una tarea concreta. Separado de la comprobación para mantener
        // las responsabilidades separadas y construir métodos escalables.
        public async Task<Result> AsignarEtiquetaATareaASync(
            Tarea tarea,
            List<Etiqueta> etiquetas
        )
        {
            foreach (var etiqueta in etiquetas)
            {
                // Creamos la relación
                await _repoTareaEtiqueta.CrearTareaEtiquetaAsync(new TareaEtiqueta
                {
                    Tarea = tarea,
                    Etiqueta = etiqueta
                });          
            }

            // Return sin guardar cambios porque el servicio lo guarda todo mediante la transacción
            return Result.Bien();       
        }
    }
}