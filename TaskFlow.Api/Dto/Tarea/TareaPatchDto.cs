namespace TaskFlow.Api.Dto.Tarea
{
    public class TareaResumenDto
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public string Estado {get; set;} = string.Empty;
        public string Prioridad {get; set;} = string.Empty;
        public DateTimeOffset FechaLimite {get; set;}
    }
}