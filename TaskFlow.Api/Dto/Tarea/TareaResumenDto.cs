using TaskFlow.Core.Enums;

namespace TaskFlow.Api.Dto.Tarea
{
    public class TareaPatchDto
    {
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
        public DateTime FechaLimite {get; set;}
        public int AsignadoId {get; set;}
    }
}