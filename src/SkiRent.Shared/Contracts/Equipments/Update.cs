using System.Text.Json.Serialization;

namespace SkiRent.Shared.Contracts.Equipments
{
    public record UpdateEquipmentRequest
    {
        public string? Name { get; init; }

        public string? Description { get; init; }

        public int? CategoryId { get; init; }

        public decimal? PricePerDay { get; init; }

        public int? AvailableQuantity { get; init; }

        [JsonIgnore]
        public string NameAsNonNull => Name ?? string.Empty;

        [JsonIgnore]
        public string DescriptionAsNonNull => Description ?? string.Empty;

        [JsonIgnore]
        public int CategoryIdAsNonNull => CategoryId ?? -1;

        [JsonIgnore]
        public decimal PricePerDayAsNonNull => PricePerDay ?? -1;

        [JsonIgnore]
        public int AvailableQuantityAsNonNull => AvailableQuantity ?? -1;
    }
}
