using TaskFlow.Core.Enums;
namespace TaskFlow.Api.Dto.ProyectoUsuario
{
    public class ProyectoUsuarioWriteDto
    {
        public int UsuarioId {get; set;}
        public int ProyectoId {get; set;}
        public RolProyecto Rol {get; set;}
    }
}