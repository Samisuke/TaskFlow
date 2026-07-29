namespace TaskFlow.Api.Dto.Tarea
{
    public class TareaReadDto
    {
        public int Id {get; set;}
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public string Estado {get; set;} = string.Empty;
        public string Prioridad {get; set;} = string.Empty;
        public DateTime FechaCreacion {get; set;}
        public DateTimeOffset FechaLimite {get; set;}
        public TareaReadDtoUsuarioAsignado Asignado {get; set;} = null!;
        public TareaReadDtoUsuarioCreador Creador {get; set;} = null!;
        public TareaReadDtoProyecto Proyecto {get; set;} = null!;
    }

    public class TareaReadDtoUsuarioAsignado
    {
        public string Nombre {get; set;} = string.Empty;
    }

    public class TareaReadDtoUsuarioCreador
    {
        public string Nombre {get; set;} = string.Empty;
    }

    public class TareaReadDtoProyecto
    {
        public string Nombre {get; set;} = string.Empty;
    }
}