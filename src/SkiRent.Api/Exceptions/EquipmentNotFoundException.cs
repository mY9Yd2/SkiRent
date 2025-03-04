namespace SkiRent.Api.Exceptions;

public class EquipmentNotFoundException : Exception
{
    public EquipmentNotFoundException()
    { }

    public EquipmentNotFoundException(string message) : base(message)
    { }

    public EquipmentNotFoundException(string message, Exception inner) : base(message, inner)
    { }
}
