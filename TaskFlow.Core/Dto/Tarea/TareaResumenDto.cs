using TaskFlow.Core.Enums;

namespace TaskFlow.Core.Dto.Tarea
{
    public class TareaResumenDto
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
        public DateTimeOffset FechaLimite {get; set;}
        public int AsignadoId {get; set;}
    }
}