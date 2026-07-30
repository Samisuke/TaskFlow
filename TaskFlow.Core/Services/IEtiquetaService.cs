using TaskFlow.Core.Common;
using TaskFlow.Core.Models;


namespace TaskFlow.Core.Services
{
    public interface IEtiquetaService
    {
        // Peticiones GET
        Task<Result<IEnumerable<Etiqueta>>> GetTodasLasEtiquetasDeUnaTareaAsync(int idTarea);
        Task<Result<Etiqueta>> GetEtiquetaPorIdAsync(int idEtiqueta);

        //Peticiones POST
        Task<Result<Etiqueta>> PostEtiquetaAsync(
            string nombreEtiqueta,
            string colorEtiqueta
        );
    }
}