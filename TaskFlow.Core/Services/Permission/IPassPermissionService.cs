using TaskFlow.Core.Models;
using TaskFlow.Core.Common;

// Define las comprobaciones relacionadas con las contraseñas de los usuarios.

namespace TaskFlow.Core.Services
{
    public interface IPassPermissionService
    {
        Result ComprobacionesPass(Usuario usuario, string passAntigua, string passNueva);
    }
}