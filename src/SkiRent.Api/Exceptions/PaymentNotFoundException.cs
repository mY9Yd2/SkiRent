namespace SkiRent.Api.Exceptions;

public class PaymentNotFoundException : Exception
{
    public PaymentNotFoundException()
    { }

    public PaymentNotFoundException(string message) : base(message)
    { }

    public PaymentNotFoundException(string message, Exception inner) : base(message, inner)
    { }
}
