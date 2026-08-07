using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con las etiquetas.

namespace TaskFlow.Core.Services
{
    public interface IEtiquetaService
    {
        // Métodos GET
        Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int etiquetaId);
    }
}