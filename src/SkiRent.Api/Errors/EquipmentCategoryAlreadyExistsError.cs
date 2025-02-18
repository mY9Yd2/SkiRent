using FluentResults;

namespace SkiRent.Api.Errors;

public class EquipmentCategoryAlreadyExistsError : Error
{
    public EquipmentCategoryAlreadyExistsError(string categoryName)
        : base($"Equipment category with name '{categoryName}' already exists.")
    {
        Metadata.Add(nameof(categoryName), categoryName);
    }
}
