namespace SkiRent.Desktop.Exceptions
{
    public class CurrentUserNotFoundException : Exception
    {
        public CurrentUserNotFoundException()
        { }

        public CurrentUserNotFoundException(string message) : base(message)
        { }

        public CurrentUserNotFoundException(string message, Exception inner) : base(message, inner)
        { }
    }
}
