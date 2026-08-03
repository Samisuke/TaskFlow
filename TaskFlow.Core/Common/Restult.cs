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