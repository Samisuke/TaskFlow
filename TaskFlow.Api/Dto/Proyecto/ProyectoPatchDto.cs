namespace TaskFlow.Api.Dto.Proyecto
{
    public class ProyectoPatchDto
    {
        public string? Nombre {get; set;} = string.Empty;
        public string? Descripcion {get; set;} = string.Empty;
        public int? PropietarioId {get; set;}
    }
}