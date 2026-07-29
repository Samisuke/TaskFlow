using TaskFlow.Core.Models;

namespace Taskflow.Core.Repositories
{
    public interface IProyectoRepository
    {
        // Obtención de Proyectos
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnUsuarioAsync(int idUsuario);
        Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int idCreador);
        Task <Proyecto?> ObtenerProyectoPorIdAsync(int idProyecto);

        // Misc.
        Task CrearProyectoAsync(Proyecto proyecto);
        Task<bool> GuardarCambiosASync();
    }
}