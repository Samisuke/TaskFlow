using TaskFlow.Core.Common;
using TaskFlow.Core.Models;

// Define la lógica de negocio relacionada con los proyectos.

namespace TaskFlow.Core.Services
{
    public interface IProyectoService
    {
        // Métodos GET
        Task<Result<Proyecto>> GetProyectoPorIdAsync(int id);
        Task<Result<IEnumerable<Proyecto>>> GetProyectosPorIdUsuarioAsync(int usuarioId);
        Task<Result<IEnumerable<Proyecto>>> GetProyectosDeUnCreadorAsync(int creadorId);

        // Métodos POST
        Task<Result<Proyecto>> PostProyectoAsync(
        string nombreProyecto,
        string descripcionProyecto,
        int PropietarioId
        );

        // Métodos PATCH
        Task<Result<Proyecto>> PatchProyectoAsync(
        int propiaId,
        int proyectoId,
        string? nombreProyecto,
        string? descripcionProyecto
        );
        Task<Result<Proyecto>> PatchDueñoProyectoAsync(
        int propiaId,
        int proyectoId,
        int PropietarioId
        );
    }
}