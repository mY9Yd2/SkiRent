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

    public async Task<Result<CreateEquipmentResponse>> CreateAsync(CreateEquipmentRequest request)
    {
        if (!await _unitOfWork.EquipmentCategories.ExistsAsync(category => category.Id == request.CategoryId))
        {
            return Result.Fail(new EquipmentCategoryNotFound(request.CategoryId));
        }

        var equipment = new Equipment();

        equipment.Name = request.Name;
        equipment.Description = request.Description;
        equipment.CategoryId = request.CategoryId;
        equipment.PricePerDay = request.PricePerDay;
        equipment.AvailableQuantity = request.AvailableQuantity;

        await _unitOfWork.Equipments.AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreateEquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            CategoryId = equipment.CategoryId,
            PricePerDay = equipment.PricePerDay,
            AvailableQuantity = equipment.AvailableQuantity
        };

        return Result.Ok(result);
    }

    public async Task<Result<GetEquipmentResponse>> GetAsync(int equipmentId)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(equipmentId);

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
            AvailableQuantity = equipment.AvailableQuantity,
            PricePerDay = equipment.PricePerDay
        };

        return Result.Ok(result);
    }

    public async Task<Result<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync()
    {
        var equipments = await _unitOfWork.Equipments.GetAllAsync();

        var result = equipments.Select(equipment =>
            new GetAllEquipmentResponse
            {
                Id = equipment.Id,
                Name = equipment.Name,
                CategoryId = equipment.CategoryId,
                PricePerDay = equipment.PricePerDay,
                IsAvaiable = equipment.AvailableQuantity > 0
            });

        return Result.Ok(result);
    }
}
