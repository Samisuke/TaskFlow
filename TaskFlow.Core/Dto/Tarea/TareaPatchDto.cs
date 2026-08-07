using TaskFlow.Core.Requests;
using TaskFlow.Core.Enums;

namespace TaskFlow.Core.Dto.Tarea
{
    public class TareaPatchDto
    {
        public string? Titulo {get; set;}
        public string? Descripcion {get; set;}
        public EstadoTarea? Estado {get; set;}
        public PrioridadTarea? Prioridad {get; set;}
        public DateTimeOffset? FechaLimite {get; set;}

        // Etiqueta
        public List<NuevaEtiqueta>? Etiquetas {get; set;} = [];
    }
}