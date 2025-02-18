namespace SkiRent.Shared.Contracts.EquipmentCategories
{
    public record UpdateEquipmentCategoryRequest
    {
        public required string Name { get; init; }
    }
}
