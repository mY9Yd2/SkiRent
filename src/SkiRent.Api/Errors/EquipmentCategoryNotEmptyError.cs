using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentCategoryNotEmptyError : Error
{
    public EquipmentCategoryNotEmptyError(int categoryId)
        : base($"Cannot delete category with id '{categoryId}' because it contains equipment.")
    {
        Metadata.Add(nameof(categoryId), categoryId);
    }
}
