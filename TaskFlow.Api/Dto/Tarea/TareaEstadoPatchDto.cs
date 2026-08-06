using TaskFlow.Core.Requests;
using TaskFlow.Core.Enums;

namespace TaskFlow.Api.Dto.Tarea
{
    public class TareaEstadoPatchDto
    {
        public int IdTarea {get; set;}
        public EstadoTarea Estado {get; set;}

        // Etiqueta
        public List<NuevaEtiqueta> Etiquetas {get; set;} = [];
    }
}