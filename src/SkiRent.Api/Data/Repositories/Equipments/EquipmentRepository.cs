using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Equipments;

public class EquipmentRepository : BaseRepository<Equipment, int>, IEquipmentRepository
{
    public EquipmentRepository(DbContext context) : base(context)
    { }

    public async Task<Equipment?> GetEquipmentWithCategoryAsync(int equipmentId)
    {
        return await _dbSet
            .Include(equipment => equipment.Category)
            .FirstOrDefaultAsync(equipment => equipment.Id == equipmentId);
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentWithCategoryAsync()
    {
        return await _dbSet
            .Include(equipment => equipment.Category)
            .ToListAsync();
    }
}
