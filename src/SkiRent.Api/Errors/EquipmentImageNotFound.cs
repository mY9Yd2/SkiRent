using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentImageNotFound : Error
{
    public EquipmentImageNotFound(Guid imageId)
        : base($"Equipment Image with id '{imageId}' not found.")
    {
        Metadata.Add(nameof(imageId), imageId);
    }
}
