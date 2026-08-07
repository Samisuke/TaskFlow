using TaskFlow.Core.Enums;

namespace TaskFlow.Core.Dto.Proyecto
{
    public class ProyectoResumenDto
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public DateTime FechaCreacion {get; set;}
        public string NombrePropietario {get; set;} = string.Empty;
    }
}