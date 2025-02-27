namespace SkiRent.Api.Exceptions;

public class CreateInvoiceRequestNotFoundException : Exception
{
    public CreateInvoiceRequestNotFoundException()
    { }

    public CreateInvoiceRequestNotFoundException(string message) : base(message)
    { }

    public CreateInvoiceRequestNotFoundException(string message, Exception inner) : base(message, inner)
    { }
}
