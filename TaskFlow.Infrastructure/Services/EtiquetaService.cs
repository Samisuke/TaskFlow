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
        public async Task<Result<IEnumerable<Etiqueta>>> GetTodasLasEtiquetasDeUnaTareaAsync(int idTarea)
        {
            var etiquetas = await _repoEtiqueta.ObtenerTodasLasEtiquetasDeUnaTareaAsync(idTarea);
            if(!etiquetas.Any()) return Result<IEnumerable<Etiqueta>>.Mal("La tarea no contiene etiquetas.");

            return Result<IEnumerable<Etiqueta>>.Bien(etiquetas);
        }

        public async Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int idEtiqueta)
        {
            var etiqueta = await _repoEtiqueta.ObtenerUnaEtiquetaPorIdAsync(idEtiqueta);
            if (etiqueta is null) return Result<Etiqueta>.Mal("La etiqueta no existe");

            return Result<Etiqueta>.Bien(etiqueta);
            
        }

        //Peticiones POST
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