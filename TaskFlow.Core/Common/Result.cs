// Notas para un posible reclutador:
//
// Clase utilizada para representar de forma uniforme el resultado de las
// operaciones realizadas por los servicios.
//
// Permite indicar:
//  - Si la operación se ha realizado correctamente.
//  - El valor devuelto cuando la operación tiene éxito.
//  - El mensaje de error cuando la operación falla.
//
// Se utiliza como contrato entre los servicios y los controllers, evitando
// depender de excepciones para controlar errores esperados de la lógica
// de negocio.
//
// También permite mantener los servicios independientes de HTTP, ya que
// no devuelven directamente códigos de estado ni tipos propios de ASP.NET Core.
//
// Existen versiones genéricas y no genéricas para operaciones que necesitan
// devolver un valor y para aquellas que únicamente necesitan indicar si la
// operación ha sido correcta.

namespace TaskFlow.Core.Common
{
    public class Result<T>
    {
        public bool EsCorrecto {get;}
        public string MensajeError {get;}
        public T? Valor {get;}

        public Result (bool esCorrecto, string mensajeError, T? valor)
        {
            EsCorrecto = esCorrecto;
            MensajeError = mensajeError;
            Valor = valor;
        }

        public static Result<T> Bien(T valor) 
            => new Result<T>(true, string.Empty, valor);

        public static Result<T> Mal(string mensajeError) 
            => new Result<T>(false, mensajeError, default);
    }

    public class Result
    {
        public bool EsCorrecto { get; init; }
        public string Error { get; init; } = string.Empty;

        public static Result Bien() => new() { EsCorrecto = true };

        public static Result Mal(string error) =>
            new()
            {
                EsCorrecto = false,
                Error = error
            };
    }
}