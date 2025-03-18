using FluentResults;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Api.Services.Equipments;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatedEquipmentResponse>> CreateAsync(CreateEquipmentRequest request)
    {
        if (!await _unitOfWork.EquipmentCategories.ExistsAsync(category => category.Id == request.CategoryId))
        {
            return Result.Fail(new EquipmentCategoryNotFound(request.CategoryId));
        }

        if (request.MainImageId is not null
            && !await _unitOfWork.EquipmentImages.ExistsAsync(image => image.Id == request.MainImageId))
        {
            return Result.Fail(new EquipmentImageNotFound((Guid)request.MainImageId));
        }

        var equipment = new Equipment
        {
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            PricePerDay = request.PricePerDay,
            AvailableQuantity = request.AvailableQuantity,
            MainImageId = request.MainImageId
        };

        await _unitOfWork.Equipments.AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreatedEquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            CategoryId = equipment.CategoryId,
            PricePerDay = equipment.PricePerDay,
            AvailableQuantity = equipment.AvailableQuantity,
            MainImageId = equipment.MainImageId
        };

        return Result.Ok(result);
    }

    public async Task<Result<GetEquipmentResponse>> GetAsync(int equipmentId)
    {
        var equipment = await _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId);

        if (equipment is null)
        {
            return Result.Fail(new EquipmentNotFoundError(equipmentId));
        }

        var result = new GetEquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            CategoryId = equipment.CategoryId,
            CategoryName = equipment.Category.Name,
            AvailableQuantity = equipment.AvailableQuantity,
            PricePerDay = equipment.PricePerDay,
            MainImageId = equipment.MainImageId
        };

        return Result.Ok(result);
    }

    public async Task<Result<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync()
    {
        var equipments = await _unitOfWork.Equipments.GetAllEquipmentWithCategoryAsync();

        var result = equipments.Select(equipment =>
            new GetAllEquipmentResponse
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                CategoryId = equipment.CategoryId,
                CategoryName = equipment.Category.Name,
                PricePerDay = equipment.PricePerDay,
                AvailableQuantity = equipment.AvailableQuantity,
                MainImageId = equipment.MainImageId
            });

        return Result.Ok(result);
    }

    public async Task<Result<GetEquipmentResponse>> UpdateAsync(int equipmentId, UpdateEquipmentRequest request)
    {
        var equipment = await _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId);

        if (equipment is null)
        {
            return Result.Fail(new EquipmentNotFoundError(equipmentId));
        }

        equipment.Name = request.Name ?? equipment.Name;

        if (request.Description is not null
            && request.Description.Trim() == string.Empty)
        {
            equipment.Description = null;
        }
        else if (!string.IsNullOrWhiteSpace(request.Description))
        {
            equipment.Description = request.Description;
        }

        if (request.CategoryId is not null)
        {
            var category = await _unitOfWork.EquipmentCategories.GetByIdAsync(request.CategoryIdAsNonNull);
            if (category is null)
            {
                return Result.Fail(new EquipmentCategoryNotFound((int)request.CategoryId));
            }
            equipment.Category = category;
        }

        if (request.MainImageId != equipment.MainImageId)
        {
            if (request.MainImageId is null)
            {
                equipment.MainImage = null;
                equipment.MainImageId = null;
            }
            else
            {
                var image = await _unitOfWork.EquipmentImages.GetByIdAsync(request.MainImageIdAsNonNull);
                if (image is null)
                {
                    return Result.Fail(new EquipmentImageNotFound((Guid)request.MainImageId));
                }
                equipment.MainImage = image;
                equipment.MainImageId = image.Id;
            }
        }

        equipment.PricePerDay = request.PricePerDay ?? equipment.PricePerDay;
        equipment.AvailableQuantity = request.AvailableQuantity ?? equipment.AvailableQuantity;

        await _unitOfWork.SaveChangesAsync();

        var result = new GetEquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            CategoryId = equipment.CategoryId,
            CategoryName = equipment.Category.Name,
            PricePerDay = equipment.PricePerDay,
            AvailableQuantity = equipment.AvailableQuantity,
            MainImageId = equipment.MainImageId
        };

        return Result.Ok(result);
    }

    public async Task<Result> DeleteAsync(int equipmentId)
    {
        var equipment = await _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId);

        if (equipment is null)
        {
            return Result.Fail(new EquipmentNotFoundError(equipmentId));
        }

        _unitOfWork.Equipments.Delete(equipment);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
