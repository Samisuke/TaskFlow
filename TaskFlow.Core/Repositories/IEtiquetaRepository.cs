using TaskFlow.Core.Models;

namespace TaskFlow.Core.Repositories
{
    public interface IEtiquetaRepository
    {
        // GET
        Task <IEnumerable<Etiqueta>> ObtenerTodasLasEtiquetasSimplesAsync();
        Task <Etiqueta?> ObtenerUnaEtiquetaPorIdAsync(int idEtiqueta);
        Task <Etiqueta?> ObtenerEtiquetaPorNombreYColorAsync(string nombre, string color);

        // POST
        Task CrearEtiquetaAsync(Etiqueta etiqueta);

        // Misc.
        Task <bool> GuardarCambiosAsync();
    }
}