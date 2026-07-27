namespace TaskFlow.Api.Dto.Proyecto
{
    public class ProyectoReadDto
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public DateTime FechaCreacion {get; set;}
        public string NombrePropietario {get; set;} = string.Empty;
        public ICollection<ProyectoReadDtoTareas> Tareas {get; set;} = null!;
        public ICollection<ProyectoReadDtoUsuarios> Usuarios {get; set;} = null!;
    }

    public class ProyectoReadDtoTareas
    {
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public string UsuarioASignado {get; set;} = string.Empty;
    }

    public class ProyectoReadDtoUsuarios
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Rol {get; set;} = string.Empty;
        public DateTime FechaIncorporacion {get; set;}
    }
}