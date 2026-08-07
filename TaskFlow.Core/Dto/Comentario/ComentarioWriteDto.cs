namespace TaskFlow.Core.Dto.Comentario
{
    public class ComentarioWriteDto
    {
        public string Contenido {get; set;} = string.Empty;
        public int TareaId {get; set;}
    }
}