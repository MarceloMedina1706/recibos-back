namespace Template.Exceptions
{
    [Serializable]
    public class RecibosPreviosException : Exception
    {
        public RecibosPreviosException() { }

        public RecibosPreviosException(string message)
            : base(message) { }

        public RecibosPreviosException(string message, Exception inner)
            : base(message, inner) { }
    }
}
