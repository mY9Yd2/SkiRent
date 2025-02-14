using FluentResults;

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
                PricePerDay = equipment.PricePerDay
            });

        return Result.Ok(result);
    }
}
