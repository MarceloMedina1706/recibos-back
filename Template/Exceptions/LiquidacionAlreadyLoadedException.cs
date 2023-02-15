namespace Template.Exceptions
{
    [Serializable]
    public class LiquidacionAlreadyLoadedException: Exception
    {
        public LiquidacionAlreadyLoadedException() { }

        public LiquidacionAlreadyLoadedException(string message)
            : base(message) { }

        public LiquidacionAlreadyLoadedException(string message, Exception inner)
            : base(message, inner) { }
    }
}
