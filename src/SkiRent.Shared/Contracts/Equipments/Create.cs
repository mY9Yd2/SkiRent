using System.Text.Json.Serialization;

namespace SkiRent.Shared.Contracts.Equipments
{
    public record CreateEquipmentRequest
    {
        public required string Name { get; init; }

        public string? Description { get; init; }

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }

        public required int AvailableQuantity { get; init; }

        public required Guid? MainImageId { get; init; }

        [JsonIgnore]
        public string DescriptionAsNonNull => Description ?? string.Empty;
    }

    public record CreatedEquipmentResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string? Description { get; init; }

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }

        public required int AvailableQuantity { get; init; }

        public required Guid? MainImageId { get; init; }
    }
}
