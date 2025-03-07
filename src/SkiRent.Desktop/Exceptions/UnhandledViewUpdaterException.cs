namespace SkiRent.Desktop.Exceptions
{
    public class UnhandledViewUpdaterException : Exception
    {
        public UnhandledViewUpdaterException()
        { }

        public UnhandledViewUpdaterException(string message) : base(message)
        { }

        public UnhandledViewUpdaterException(string message, Exception inner) : base(message, inner)
        { }
    }
}
