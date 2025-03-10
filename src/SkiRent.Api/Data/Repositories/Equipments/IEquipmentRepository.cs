using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Equipments;

public interface IEquipmentRepository : IRepository<Equipment, int>
{
    public Task<Equipment?> GetEquipmentWithCategoryAsync(int equipmentId);
    public Task<IEnumerable<Equipment>> GetAllEquipmentWithCategoryAsync();
}
