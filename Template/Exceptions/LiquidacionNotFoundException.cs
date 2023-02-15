namespace Template.Exceptions
{
    [Serializable]
    public class LiquidacionNotFoundException : Exception
    {
        public LiquidacionNotFoundException() { }

        public LiquidacionNotFoundException(string message)
            : base(message) { }

        public LiquidacionNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
