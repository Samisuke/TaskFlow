// Representa un proyecto que puede contener varias tareas.
// Puede ser creado por un usuario y dar acceso a otros. Dispone de una colección de tareas dentro del proyecto
// y una colección de usuarios con acceso al proyecto, que a su vez tiene la información de cada uno de ellos.

namespace TaskFlow.Core.Models
{
    public class Proyecto
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public DateTime FechaCreacion {get; set;}
        public int PropietarioId {get; set;}
        public Usuario? Propietario { get; set; }

        public ICollection<Tarea> Tareas { get; set; } = [];
        public ICollection<ProyectoUsuario> Usuarios {get; set;} = [];
        public ICollection<Historial> Historiales {get; set;} = [];
    }
}