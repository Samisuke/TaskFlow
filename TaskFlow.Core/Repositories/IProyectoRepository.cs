using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IProyectoRepository
    {
        // GET
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnUsuarioAsync(int idUsuario);
        Task <IEnumerable<Proyecto>> ObtenerProyectosPropiosAsync(int idUsuario);
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int idCreador);
        Task <Proyecto?> ObtenerProyectoPorIdAsync(int idProyecto);

        // POST
        Task CrearProyectoAsync(Proyecto proyecto);

        // Misc.
        Task<bool> GuardarCambiosASync();
    }
}