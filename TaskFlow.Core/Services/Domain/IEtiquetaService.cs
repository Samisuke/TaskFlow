using TaskFlow.Core.Common;
using TaskFlow.Core.Models;


namespace TaskFlow.Core.Services
{
    public interface IEtiquetaService
    {
        // Métodos GET
        Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int idEtiqueta);
    }
}