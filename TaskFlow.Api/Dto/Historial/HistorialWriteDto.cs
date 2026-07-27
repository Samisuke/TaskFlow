namespace TaskFlow.Api.Dto.Historial
{
    public class HistorialWriteDto
    {
        public int TareaId {get; set;}
        public int UsuarioId {get; set;}
        public string Accion {get; set;} = string.Empty;
    }
}