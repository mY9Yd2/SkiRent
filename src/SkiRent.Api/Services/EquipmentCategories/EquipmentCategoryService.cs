using FluentResults;

using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Services.EquipmentCategories;

public class EquipmentCategoryService : IEquipmentCategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentCategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<GetAllEquipmentCategoryResponse>>> GetAllAsync()
    {
        var equipmentCategories = await _unitOfWork.EquipmentCategories.GetAllAsync();

        var result = equipmentCategories.Select(equipment =>
            new GetAllEquipmentCategoryResponse
            {
                Id = equipment.Id,
                Name = equipment.Name
            });

        return Result.Ok(result);
    }
}
