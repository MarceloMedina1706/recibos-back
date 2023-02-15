namespace Template.Exceptions
{
    [Serializable]
    public class EmpleadoNotFoundException : Exception
    {
        public EmpleadoNotFoundException() { }

        public EmpleadoNotFoundException(string message)
            : base(message) { }

        public EmpleadoNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
