namespace SkiRent.Shared.Contracts.EquipmentCategories
{
    public record GetEquipmentCategoryResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }
    }
}
