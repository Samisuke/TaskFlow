using TaskFlow.Core.Enums;
using TaskFlow.Core.Requests;

namespace TaskFlow.Core.Dto.Tarea
{
    public class TareaWriteDto
    {
        // Tarea
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
        public DateTimeOffset FechaLimite {get; set;}
        public int ProyectoId {get; set;}
        public int AsignadoId {get; set;}

        // Etiqueta
        public List<NuevaEtiqueta> Etiquetas {get; set;} = [];
    }
}