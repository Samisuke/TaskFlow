// Representa un usuario de la aplicación.
// Puede ser creado por registro y logeado. Contiene una colección de proyectos, desde la que
// se puede acceder a su vez a la información de tareas, comentarios...

namespace TaskFlow.Core.Models
{
    public class Usuario
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Apellidos {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PasswordHash {get; set;} = string.Empty;
        public DateTime FechaRegistro {get; set;}
        public bool Activo {get; set;}

        public ICollection<ProyectoUsuario> Proyectos {get; set;} = [];
        public ICollection<Comentario> Comentarios {get; set;} = [];
        public ICollection<Tarea>? TareasAsignadas {get; set;}
        public ICollection<Tarea>? TareasCreadas {get; set;}
    }
}