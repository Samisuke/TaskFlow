using TaskFlow.Core.Enums;

namespace TaskFlow.Core.Dto.ProyectoUsuario
{
    public class ProyectoUsuarioPatchDto
    {
        public bool? Activo {get; set;}
        public RolProyecto? Rol {get; set;}
    }
}