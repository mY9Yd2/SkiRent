using FluentResults;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Services.EquipmentCategories;

public class EquipmentCategoryService : IEquipmentCategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentCategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatedEquipmentCategoryResponse>> CreateAsync(CreateEquipmentCategoryRequest request)
    {
        if (await _unitOfWork.EquipmentCategories.ExistsAsync(category => category.Name == request.Name))
        {
            return Result.Fail(new EquipmentCategoryAlreadyExistsError(request.Name));
        }

        var category = new Equipmentcategory();

        category.Name = request.Name;

        await _unitOfWork.EquipmentCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreatedEquipmentCategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };

        return Result.Ok(result);
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
