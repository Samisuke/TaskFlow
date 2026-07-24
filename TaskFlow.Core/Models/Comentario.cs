// Representa un comentario dentro de una tarea.
// Puede ser asignada a una tarea por un usuario.
// Dispone de informacion sobre la tarea a la que pertenece y el usuario que la creó.

namespace TaskFlow.Core.Models
{
    public class Comentario
    {
        public int Id {get; set;}
        public string Contenido {get; set;} = string.Empty;
        public DateTime Fecha {get; set;}
        public int UsuarioId {get; set;}
        public Usuario? Usuario {get; set;}
        public int TareaId {get; set;}
        public Tarea? Tarea {get; set;}
    }
}