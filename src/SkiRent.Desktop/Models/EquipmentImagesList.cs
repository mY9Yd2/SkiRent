namespace SkiRent.Desktop.Models
{
    public record EquipmentImagesList
    {
        public required Guid Id { get; init; }

        public required string? DisplayName { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public required Uri ImageUrl { get; init; }
    }
}
