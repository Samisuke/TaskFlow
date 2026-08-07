// DTO utilizado para representar una etiqueta proporcionada por el cliente.
// Se utiliza al crear o asignar etiquetas a una tarea.

namespace TaskFlow.Core.Requests
{
    public class NuevaEtiqueta
    {
        public string Nombre {get; set;} = string.Empty;
        public string Color {get; set;} = string.Empty;
    }
}