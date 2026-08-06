using TaskFlow.Core.Models;
using TaskFlow.Core.Common;

namespace TaskFlow.Core.Services
{
    public interface IPassPermissionService
    {
        Result ComprobacionesPass(Usuario usuario, string passAntigua, string passNueva);
    }
}