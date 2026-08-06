namespace TaskFlow.Core.Services.Token
{
    public interface ITokenService
    {
        string GenerarToken(int id, string nombre, string email);
    }
}