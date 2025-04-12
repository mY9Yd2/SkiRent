using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentImageNotFoundError : Error
{
    public EquipmentImageNotFoundError(Guid imageId)
        : base($"Equipment Image with id '{imageId}' not found.")
    {
        Metadata.Add(nameof(imageId), imageId);
    }
}
