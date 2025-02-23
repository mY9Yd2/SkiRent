using FluentResults;

namespace SkiRent.Api.Errors;

public class InsufficientQuantityError : Error
{
    public InsufficientQuantityError(int equipmentId)
        : base($"Insufficient quantity for equipment with id '{equipmentId}'.")
    {
        Metadata.Add(nameof(equipmentId), equipmentId);
    }
}
