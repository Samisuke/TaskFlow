using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;

namespace TaskFlow.Infrastructure.Services
{
    public class EtiquetaService : IEtiquetaService
    {
        // Inyección del repositorio
        private readonly IEtiquetaRepository _repoEtiqueta;

        public EtiquetaService(IEtiquetaRepository repoEtiqueta)
        {
            _repoEtiqueta = repoEtiqueta;
        }

        // Peticiones GET
        // Obtener todas las etiquetas de una tarea. Útil para poder mostrarlas de forma rápida
        public async Task<Result<IEnumerable<Etiqueta>>> GetTodasLasEtiquetasDeUnaTareaAsync(int idTarea)
        {
            var etiquetas = await _repoEtiqueta.ObtenerTodasLasEtiquetasDeUnaTareaAsync(idTarea);
            if(!etiquetas.Any()) return Result<IEnumerable<Etiqueta>>.Mal("La tarea no contiene etiquetas.");

            return Result<IEnumerable<Etiqueta>>.Bien(etiquetas);
        }

        // Obtener una etiqueta. Útil si quieres mostrar una etiqueta como sugerencia, por ejemplo.
        public async Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int idEtiqueta)
        {
            var etiqueta = await _repoEtiqueta.ObtenerUnaEtiquetaPorIdAsync(idEtiqueta);
            if (etiqueta is null) return Result<Etiqueta>.Mal("La etiqueta no existe");

            return Result<Etiqueta>.Bien(etiqueta);
            
        }

        //Peticiones POST
        // Crear una etiqueta.
        public async Task<Result<Etiqueta>> PostEtiquetaAsync(
            string nombreEtiqueta,
            string colorEtiqueta
        )
        {
            var etiqueta = new Etiqueta
            {
                Nombre = nombreEtiqueta,
                Color = colorEtiqueta
            };

            await _repoEtiqueta.CrearEtiquetaAsync(etiqueta);
            var guardadoExitoso = await _repoEtiqueta.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Etiqueta>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Etiqueta>.Bien(etiqueta);
        }
    }
}