using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

namespace TaskFlow.Core.Services
{
    public interface IProyectoService
    {
        // Métodos GET
        Task<Result<Proyecto>> GetProyectoPorIdAsync(int id);
        Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnaPersonaAsync(int idUsuario);
        Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnCreadorAsync(int idCreador);

        // Métodos POST
        Task<Result<Proyecto>> PostProyectoAsync(
        string nombreProyecto,
        string descripcionProyecto,
        int PropietarioId
        );

        // Métodos PATCH
        Task<Result<Proyecto>> PatchProyectoAsync(
        int idPropia,
        int idProyecto,
        string? nombreProyecto,
        string? descripcionProyecto
        );
        Task<Result<Proyecto>> PatchDueñoProyectoAsync(
        int idPropia,
        int idProyecto,
        int PropietarioId
        );
    }
}