using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IEtiquetaRepository
    {
        // Obtención de Etiquetas
        Task <IEnumerable<Etiqueta>> ObtenerTodasLasEtiquetasSimplesAsync();
        Task <IEnumerable<Etiqueta>> ObtenerTodasLasEtiquetasDeUnaTareaAsync(int idTarea);
        Task <Etiqueta?> ObtenerUnaEtiquetaPorIdAsync(int idEtiqueta);

        // Misc.
        Task CrearEtiquetaAsync(Etiqueta etiqueta);
        Task <bool> GuardarCambiosAsync();
    }
}