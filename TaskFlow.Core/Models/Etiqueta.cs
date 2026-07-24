// Representa una etiqueta dentro de una tarea.
// Puede ser asignada a una tarea por un usuario.
// Dispone de informacion sobre la tarea a la que pertenece mediante la colección "Tareas".

namespace TaskFlow.Core.Models
{
    public class Etiqueta
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Color {get; set;} = string.Empty;

        public ICollection<TareaEtiqueta> Tareas { get; set; } = [];
    }
}