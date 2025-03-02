using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Equipments;

public class EquipmentRepository : BaseRepository<Equipment, int>, IEquipmentRepository
{
    public EquipmentRepository(DbContext context) : base(context)
    { }
}
