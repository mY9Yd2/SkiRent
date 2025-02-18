namespace SkiRent.Shared.Contracts.EquipmentCategories
{
    public record CreateEquipmentCategoryRequest
    {
        public required string Name { get; init; }
    }

    public record CreatedEquipmentCategoryResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }
    }
}
