// Representa una lista intermedia. Es el nexo entre Tarea y Etiquetas.
// Contiene información sobre las etiquetas y las tareas para poder disponer de ella
// mediante una coleccion en cada tarea y etiqueta.

namespace TaskFlow.Core.Models
{
    public class TareaEtiqueta
    {
        public int TareaId {get; set;}
        public Tarea? Tarea {get; set;}
        public int EtiquetaId {get; set;}
        public Etiqueta? Etiqueta {get; set;}
    }
}