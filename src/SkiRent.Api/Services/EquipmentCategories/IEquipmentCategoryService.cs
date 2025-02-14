using FluentResults;

using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Services.EquipmentCategories;

public interface IEquipmentCategoryService
{
    public Task<Result<IEnumerable<GetAllEquipmentCategoryResponse>>> GetAllAsync();
}
