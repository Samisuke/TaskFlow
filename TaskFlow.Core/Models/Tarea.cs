using TaskFlow.Core.Enums;

// Representa una tarea dentro de un proyecto.
// Puede ser asignada de un usuario a otro y disponer de comentarios, etiquetas e historial de cambios.
// Dispone de informacion sobre el proyecto al que pertenece, el usuario asignado y el creador de la misma.

namespace TaskFlow.Core.Models
{
    public class Tarea
    {
        public int Id {get; set;}
        public string Titulo {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public EstadoTarea Estado {get; set;}
        public PrioridadTarea Prioridad {get; set;}
        public DateTime FechaCreacion {get; set;}
        public DateTime FechaLimite {get; set;}
        public int ProyectoId {get; set;}
        public Proyecto? Proyecto {get; set;}
        public int AsignadoId {get; set;}
        public Usuario? Asignado {get; set;}
        public int CreadorId {get; set;}
        public Usuario? Creador {get; set;}

        public ICollection<Comentario> Comentarios {get; set;}  = [];
        public ICollection<Historial> Historiales {get; set;} = [];
        public ICollection<TareaEtiqueta> Etiquetas {get; set;} = [];
    }
}