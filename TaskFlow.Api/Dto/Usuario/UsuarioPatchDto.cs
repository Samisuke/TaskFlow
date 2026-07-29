namespace TaskFlow.Api.Dto.Usuario
{
    public class UsuarioPatchDto
    {
        public string Nombre {get; set;} = string.Empty;
        public string Apellidos {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public bool Activo {get; set;}
    }
}