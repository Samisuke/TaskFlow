namespace TaskFlow.Api.Dto.Comentario
{
    public class ComentarioResumenDto
    {
        public int Id {get; set;}
        public string Usuario {get; set;} = string.Empty;
        public string Contenido {get; set;} = string.Empty;
        public DateTime Fecha {get; set;}
    }
}