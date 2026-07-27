namespace TaskFlow.Api.Dto.Proyecto
{
    public class ProyectoWriteDto
    {
        public string Nombre {get; set;} = string.Empty;
        public string Descripcion {get; set;} = string.Empty;
        public int PropietarioId {get; set;}
    }
}