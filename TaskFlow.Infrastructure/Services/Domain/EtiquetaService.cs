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
        // Obtener una etiqueta. Útil si quieres mostrar una etiqueta como sugerencia, por ejemplo.
        public async Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int etiquetaId)
        {
            var etiqueta = await _repoEtiqueta.ObtenerUnaEtiquetaPorIdAsync(etiquetaId);
            if (etiqueta is null) return Result<Etiqueta>.Mal("La etiqueta no existe");

            return Result<Etiqueta>.Bien(etiqueta);
        }
    }
}