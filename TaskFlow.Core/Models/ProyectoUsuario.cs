using TaskFlow.Core.Enums;

// Representa una tabla intermedia. Es el nexo entre Usuario y Proyecto.
// Contiene información sobre los usarios y el proyecto para poder acceder a ella desde una colección
// añadida a cada Usuario y Proyecto.

namespace TaskFlow.Core.Models
{
    public class ProyectoUsuario
    {
        public int UsuarioId {get; set;}
        public Usuario? Usuario {get; set;} = null!;
        public DateTime FechaIncorporacion {get; set;}
        public int ProyectoId {get; set;}
        public Proyecto? Proyecto {get; set;}
        public RolProyecto Rol {get; set;}
        public bool Activo {get; set;}
    }
}