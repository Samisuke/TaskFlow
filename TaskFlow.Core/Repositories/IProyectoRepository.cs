using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IProyectoRepository
    {
        // GET
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnUsuarioAsync(int idUsuario);
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int idCreador);

        // Misc.
        Task CrearProyectoAsync(Proyecto proyecto);
        Task<bool> GuardarCambiosASync();
    }
}