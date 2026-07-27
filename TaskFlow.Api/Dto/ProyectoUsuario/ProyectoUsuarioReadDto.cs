using TaskFlow.Core.Enums;

namespace TaskFlow.Api.Dto.ProyectoUsuario
{
    public class ProyectoUsuarioReadDto
    {
        public ProyectoUsuarioReadDtoUsuario Usuario {get; set;} = null!;
        public DateTime FechaIncorporacion {get; set;}
        public string Rol {get; set;} = string.Empty;
    }

    public class ProyectoUsuarioReadDtoUsuario
    {
        public string Nombre {get; set;} = string.Empty;
        public string Apellidos {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public ICollection<ProyectoUsuarioReadDtoTareasAsignadas> TareasAsignadas {get; set;} = null!;
        public ICollection<ProeyectoUsuarioReadDtoTareasCreadas> TareasCreadas {get; set;} = null!;
    }
    public class ProyectoUsuarioReadDtoTareasAsignadas
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
    }

    public class ProeyectoUsuarioReadDtoTareasCreadas
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
    }
}