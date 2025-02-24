namespace SkiRent.FakePay.Models;

public record Item
{
    public required string Name { get; init; }

    public required string SubText { get; init; }

    public required decimal TotalPrice { get; init; }
}
