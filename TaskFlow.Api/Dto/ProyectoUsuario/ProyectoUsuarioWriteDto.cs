using TaskFlow.Core.Enums;
namespace TaskFlow.Api.Dto.ProyectoUsuario
{
    public class ProyetoUsuarioWriteDto
    {
        public int UsuarioId {get; set;}
        public int ProyectoId {get; set;}
        public RolProyecto Rol {get; set;}
    }
}