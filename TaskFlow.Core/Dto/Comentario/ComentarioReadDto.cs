namespace TaskFlow.Core.Dto.Comentario
{
    public class ComentarioReadDto
    {
        public int Id {get; set;}
        public string Contenido {get; set;} = string.Empty;
        public DateTime Fecha {get; set;}
        public ComentarioReadDtoUsuario Usuario {get; set;} = null!;
        public ComentarioReadDtoTarea Tarea {get; set;} = null!;
    }

    public class ComentarioReadDtoUsuario
    {
        public string NombreUsuario {get; set;} = string.Empty;
    }

        public class ComentarioReadDtoTarea
    {
        public string NombreTarea {get; set;} = string.Empty;
    }
}