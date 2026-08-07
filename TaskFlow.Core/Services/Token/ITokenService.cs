// Define las operaciones relacionadas con la generación de tokens JWT para la autenticación de usuarios.

namespace TaskFlow.Core.Services.Token
{
    public interface ITokenService
    {
        string GenerarToken(int id, string nombre, string email);
    }
}