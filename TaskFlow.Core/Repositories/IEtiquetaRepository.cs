using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con las etiquetas.

namespace TaskFlow.Core.Repositories
{
    public interface IEtiquetaRepository
    {
        // GET
        Task <Etiqueta?> ObtenerUnaEtiquetaPorIdAsync(int etiquetaId);
        Task <Etiqueta?> ObtenerEtiquetaPorNombreYColorAsync(string nombre, string color);

        // POST
        Task CrearEtiquetaAsync(Etiqueta etiqueta);
    }
}