namespace SkiRent.Shared.Contracts.EquipmentImages
{
    public record GetAllEquipmentImageResponse
    {
        public required Guid Id { get; init; }

        public required string? DisplayName { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public required DateTimeOffset UpdatedAt { get; init; }
    }
}
