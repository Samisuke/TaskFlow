using TaskFlow.Core.Models;

// Define las operaciones de acceso y persistencia de datos relacionadas con los proyectos.

namespace TaskFlow.Core.Repositories
{
    public interface IProyectoRepository
    {
        // GET
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnUsuarioAsync(int usuarioId);
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int creadorId);
        Task <Proyecto?> ObtenerProyectoPorIdAsync(int proyectoId);

        // POST
        Task CrearProyectoAsync(Proyecto proyecto);
    }
}