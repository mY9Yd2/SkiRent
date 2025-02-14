using FluentResults;

using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Api.Services.Equipments;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync()
    {
        var equipments = await _unitOfWork.Equipments.GetAllAsync();

        var result = equipments.Select(equipment =>
            new GetAllEquipmentResponse
            {
                Name = equipment.Name,
                CategoryId = equipment.CategoryId,
                PricePerDay = equipment.PricePerDay
            });

        return Result.Ok(result);
    }
}
