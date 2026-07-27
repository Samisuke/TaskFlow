using TaskFlow.Core.Enums;
namespace TaskFlow.Api.Dto.ProyectoUsuario
{
    public class ProyetoUsuarioPathcDto
    {
        public int UsuarioId {get; set;}
        public int ProyectoId {get; set;}
        public bool Activo {get; set;}
        public RolProyecto Rol {get; set;}
    }
}