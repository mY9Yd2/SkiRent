using FluentResults;

using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Api.Services.Equipments;

public interface IEquipmentService
{
    public Task<Result<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync();
}
