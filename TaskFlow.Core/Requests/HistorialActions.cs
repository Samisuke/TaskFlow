// Con HistorialActions podemos modificar el texto que crea el historial desde aquí, sin tener que estar yendo a diferentes servicios a cambiarlo.
// Decidí centralizarlo por comodidad y escalabilidad.

namespace TaskFlow.Core.Requests
{
    public class HistorialActions
    {
        public const string ComentarioCreado = "Creó un comentario.";
        public const string ComentarioModificado = "Modificó un comentario.";
        public const string ProyectoModificado = "Modificó el proyecto.";
        public const string ProyectoDueñoModificado = "es ahora el dueño del proyecto.";
        public const string AñadirPersona = "Ha añadido a una persona al proyecto.";
        public const string ModificarPersonaEnProyecto = "Ha modificado un usuario.";
        public const string ModificarEstadoTarea = "Ha modificado el estado de una tarea.";
        public const string ModificarTarea = "Ha modificado una tarea.";
        public const string AñadirTarea = "Ha añadido una tarea.";
    }
}