namespace SkiRent.FakePay.Models;

public record Item
{
    public required string Name { get; init; }

    public required decimal Price { get; init; }

    public required int Quantity { get; init; }
}
