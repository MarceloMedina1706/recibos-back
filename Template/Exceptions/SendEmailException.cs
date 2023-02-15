namespace Template.Exceptions
{
    [Serializable]
    public class SendEmailException : Exception
    {
        public SendEmailException() { }

        public SendEmailException(string message)
            : base(message) { }

        public SendEmailException(string message, Exception inner)
            : base(message, inner) { }
    }
}
