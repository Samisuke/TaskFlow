using TaskFlow.Core.Enums;

namespace TaskFlow.Api.Dto.Tarea
{
    public class TareaWriteDto
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
        public DateTimeOffset FechaLimite {get; set;}
        public int ProyectoId {get; set;}
        public int AsignadoId {get; set;}
    }
}