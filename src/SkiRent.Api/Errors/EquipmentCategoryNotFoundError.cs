using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentCategoryNotFoundError : Error
{
    public EquipmentCategoryNotFoundError(int categoryId)
        : base($"Equipment Category with id '{categoryId}' not found.")
    {
        Metadata.Add(nameof(categoryId), categoryId);
    }
}
