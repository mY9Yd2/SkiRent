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

        var category = new EquipmentCategory();

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

    public async Task<Result<GetEquipmentCategoryResponse>> UpdateAsync(int categoryId, UpdateEquipmentCategoryRequest request)
    {
        var category = await _unitOfWork.EquipmentCategories.GetByIdAsync(categoryId);

        if (category is null)
        {
            return Result.Fail(new EquipmentCategoryNotFound(categoryId));
        }

        if (request.Name is not null)
        {
            if (await _unitOfWork.EquipmentCategories.ExistsAsync(category => category.Name == request.Name))
            {
                return Result.Fail(new EquipmentCategoryAlreadyExistsError(request.Name));
            }
            category.Name = request.Name;
        }

        await _unitOfWork.SaveChangesAsync();

        var result = new GetEquipmentCategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };

        return Result.Ok(result);
    }

    public async Task<Result> DeleteAsync(int categoryId)
    {
        var category = await _unitOfWork.EquipmentCategories.GetByIdAsync(categoryId);

        if (category is null)
        {
            return Result.Fail(new EquipmentCategoryNotFound(categoryId));
        }

        var hasEquipment = await _unitOfWork.Equipments.ExistsAsync(equipment => equipment.CategoryId == categoryId);

        if (hasEquipment)
        {
            return Result.Fail(new EquipmentCategoryNotEmptyError(categoryId));
        }

        _unitOfWork.EquipmentCategories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
