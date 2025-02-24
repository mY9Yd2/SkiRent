using System.Globalization;

namespace SkiRent.FakePay.Models;

public record Payment
{
    public required Guid Id { get; init; }

    public required string MerchantName { get; init; }

    public required IEnumerable<Item> Items { get; init; }

    public required decimal TotalPrice { get; init; }

    public required CultureInfo Culture { get; init; }

    public required string CurrencyCode { get; init; }

    public required Uri CallbackUrl { get; init; }

    public required Uri SuccessUrl { get; init; }

    public required Uri CancelUrl { get; init; }
}
