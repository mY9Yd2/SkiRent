using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentCategoryNotFound : Error
{
    public EquipmentCategoryNotFound(int categoryId)
        : base($"Equipment Category with id '{categoryId}' not found.")
    {
        Metadata.Add(nameof(categoryId), categoryId);
    }
}
