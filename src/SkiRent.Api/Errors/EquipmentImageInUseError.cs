using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentImageInUseError : Error
{
    public EquipmentImageInUseError(Guid imageId)
        : base($"Cannot delete the equipment image with id '{imageId}' because it is currently associated with one or more equipment items.")
    {
        Metadata.Add(nameof(imageId), imageId);
    }
}
