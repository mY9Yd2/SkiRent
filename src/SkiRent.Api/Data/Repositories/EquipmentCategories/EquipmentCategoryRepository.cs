using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.EquipmentCategories;

public class EquipmentCategoryRepository : BaseRepository<EquipmentCategory>, IEquipmentCategoryRepository
{
    public EquipmentCategoryRepository(DbContext context) : base(context)
    { }
}
