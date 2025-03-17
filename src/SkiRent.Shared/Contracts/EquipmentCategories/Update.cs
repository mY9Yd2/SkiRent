using System.Text.Json.Serialization;

namespace SkiRent.Shared.Contracts.EquipmentCategories
{
    public record UpdateEquipmentCategoryRequest
    {
        public string? Name { get; init; }

        [JsonIgnore]
        public string NameAsNonNull => Name ?? string.Empty;
    }
}
