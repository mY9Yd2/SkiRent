namespace SkiRent.Shared.Contracts.EquipmentCategories
{
    public record GetAllEquipmentCategoryResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }
    }
}
