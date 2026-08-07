namespace TaskFlow.Core.Dto.Historial
{
    public class HistorialReadDto
    {
        public int Id {get; set;}
        public string Accion {get; set;} = string.Empty;
        public DateTime Fecha {get; set;}
        public HistorialReadDtoUsuario Usuario {get; set;} = null!;
        public HistorialReadDtoProyecto Proyecto {get; set;} = null!;
    }

    public class HistorialReadDtoUsuario
    {
        public string Nombre {get; set;} = string.Empty;
    }

    public class HistorialReadDtoProyecto
    {
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
    }
}