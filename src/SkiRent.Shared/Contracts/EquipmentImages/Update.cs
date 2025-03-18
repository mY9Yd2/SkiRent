namespace SkiRent.Shared.Contracts.EquipmentImages
{
    public record UpdateEquipmentImageRequest
    {
        public string? DisplayName { get; init; }

        public string? Base64ImageData { get; init; }
    }
}
