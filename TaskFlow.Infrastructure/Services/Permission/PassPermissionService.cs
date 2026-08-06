using TaskFlow.Core.Services;
using TaskFlow.Core.Models;
using TaskFlow.Core.Common;
using BCrypt.Net;

namespace TaskFlow.Infrastructure.Services
{
    public class PassPermissionService : IPassPermissionService
    { 
        // Comparar contraseña introducida como "contraseña actual" con la actual para comprobar que eres tú.
        private Result CompararPass(Usuario usuario, string passAntigua)
        {
            if (!BCrypt.Net.BCrypt.Verify(passAntigua, usuario.PasswordHash)) return Result.Mal("La contraseña introducida no es correcta");
            
            return Result.Bien();
        }

        // Comparar contraseña nueva con la actual para que no sea la misma.
        private Result PassRepetida(Usuario usuario, string passNueva)
        {
            if (BCrypt.Net.BCrypt.Verify(passNueva, usuario.PasswordHash)) return Result.Mal("La nueva contraseña no puede ser la misma que la que ya tienes.");

            return Result.Bien();
        }

        // Realizar todas las comprobaciones.
        public Result ComprobacionesPass(Usuario usuario, string passAntigua, string passNueva)
        {
            var comparacion = CompararPass(usuario, passAntigua);
            if (!comparacion.EsCorrecto) return Result.Mal(comparacion.Error);

            var repetida = PassRepetida(usuario, passNueva);
            if (!repetida.EsCorrecto) return Result.Mal(repetida.Error);
            
            return Result.Bien();
        }
    }
}