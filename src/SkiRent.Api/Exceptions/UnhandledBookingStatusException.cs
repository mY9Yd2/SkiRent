namespace SkiRent.Api.Exceptions;

public class UnhandledBookingStatusException : Exception
{
    public UnhandledBookingStatusException()
    { }

    public UnhandledBookingStatusException(string message) : base(message)
    { }

    public UnhandledBookingStatusException(string message, Exception inner) : base(message, inner)
    { }
}
