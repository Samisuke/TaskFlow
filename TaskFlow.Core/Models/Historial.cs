// Representa el historial de cambios en una tarea.
// Cuando se efectua un cambio, este se registra y guarda.
// Dispone de informacion sobre el usuario que realizó el cambio y la tarea donde se efectuó.

namespace TaskFlow.Core.Models
{
    public class Historial
    {
        public int Id {get; set;}
        public int TareaId {get; set;}
        public Tarea? Tarea {get; set;}
        public string Accion {get; set;} = string.Empty;
        public DateTime Fecha {get; set;}

        public int UsuarioId {get; set;}
        public Usuario? Usuario {get; set;}
    }
}