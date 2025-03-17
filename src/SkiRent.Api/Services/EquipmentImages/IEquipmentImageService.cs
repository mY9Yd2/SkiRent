using FluentResults;

using SkiRent.Shared.Contracts.EquipmentImages;

namespace SkiRent.Api.Services.EquipmentImages;

public interface IEquipmentImageService
{
    public Task<Result<CreatedEquipmentImageResponse>> CreateAsync(IFormFile formFile);
    public Task<Result<IEnumerable<GetAllEquipmentImageResponse>>> GetAllAsync();
    public Task<Result> DeleteAsync(Guid imageId);
}
