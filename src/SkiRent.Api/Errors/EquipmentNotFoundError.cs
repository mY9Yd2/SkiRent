using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentNotFoundError : Error
{
    public EquipmentNotFoundError(int equipmentId)
        : base($"Equipment with id '{equipmentId}' not found.")
    {
        Metadata.Add(nameof(equipmentId), equipmentId);
    }
}
