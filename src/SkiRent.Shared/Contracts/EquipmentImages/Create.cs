namespace SkiRent.Shared.Contracts.EquipmentImages
{
    public record CreatedEquipmentImageResponse
    {
        public required Guid Id { get; init; }

        public required string? DisplayName { get; init; }
    }
}
