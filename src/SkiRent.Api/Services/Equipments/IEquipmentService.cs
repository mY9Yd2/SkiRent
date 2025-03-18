using FluentResults;

using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Api.Services.Equipments;

public interface IEquipmentService
{
    public Task<Result<CreatedEquipmentResponse>> CreateAsync(CreateEquipmentRequest request);
    public Task<Result<GetEquipmentResponse>> GetAsync(int equipmentId);
    public Task<Result<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync();
    public Task<Result<GetEquipmentResponse>> UpdateAsync(int equipmentId, UpdateEquipmentRequest request);
    public Task<Result> DeleteAsync(int equipmentId);
}
