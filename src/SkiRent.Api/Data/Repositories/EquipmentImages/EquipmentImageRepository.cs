using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.EquipmentImages;

public class EquipmentImageRepository : BaseRepository<EquipmentImage, Guid>, IEquipmentImageRepository
{
    public EquipmentImageRepository(SkiRentContext context) : base(context)
    { }
}
