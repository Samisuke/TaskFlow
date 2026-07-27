namespace TaskFlow.Api.Dto.Usuario
{
    public class UsuarioReadDto
    {
        public int Id {get; set;}
        public string Nombre {get; set;} = string.Empty;
        public string Apellidos {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public bool Activo {get; set;}
        public ICollection<UsuarioReadDtoProyectos> Proyectos {get; set;} = null!;
        public ICollection<UsuarioReadDtoComentarios> Comentarios {get; set;} = null!;
    }

    public class UsuarioReadDtoProyectos
    {
        public string Nombre {get; set;} = string.Empty;
    }
    
    public class UsuarioReadDtoComentarios
    {
        public string Contenido {get; set;} = string.Empty;
        public string PerteneceA{get; set;} = null!;
    }
}