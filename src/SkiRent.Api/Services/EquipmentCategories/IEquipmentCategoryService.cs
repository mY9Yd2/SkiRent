using FluentResults;

using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Services.EquipmentCategories;

public interface IEquipmentCategoryService
{
    public Task<Result<CreatedEquipmentCategoryResponse>> CreateAsync(CreateEquipmentCategoryRequest request);
    public Task<Result<IEnumerable<GetAllEquipmentCategoryResponse>>> GetAllAsync();
    public Task<Result<GetEquipmentCategoryResponse>> UpdateAsync(int categoryId, UpdateEquipmentCategoryRequest request);
    public Task<Result> DeleteAsync(int categoryId);
}
