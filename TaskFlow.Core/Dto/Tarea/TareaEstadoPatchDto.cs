using TaskFlow.Core.Requests;
using TaskFlow.Core.Enums;

namespace TaskFlow.Core.Dto.Tarea
{
    public class TareaEstadoPatchDto
    {
        public int IdTarea {get; set;}
        public EstadoTarea Estado {get; set;}
    }
}