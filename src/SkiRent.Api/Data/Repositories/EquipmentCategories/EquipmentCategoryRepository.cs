using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.EquipmentCategories;

public class EquipmentCategoryRepository : BaseRepository<Equipmentcategory>, IEquipmentCategoryRepository
{
    public EquipmentCategoryRepository(DbContext context) : base(context)
    { }
}
