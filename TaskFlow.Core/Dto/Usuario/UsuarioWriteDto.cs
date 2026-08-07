namespace TaskFlow.Core.Dto.Usuario
{
    public class UsuarioWriteDto
    {
        public string Nombre {get; set;} = string.Empty;
        public string Apellidos {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PasswordHash {get; set;} = string.Empty;
        public bool Activo {get; set;} = true; // Al crearte una cuenta, siempre nace activada.
    }
}