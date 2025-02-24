using System.Globalization;

using SkiRent.FakePay.Models;
using SkiRent.Shared.Contracts.Payments;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.FakePay.Services.Payments;

public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IFusionCache _cache;
    private readonly IHttpClientFactory _clientFactory;

    public PaymentService(ILogger<PaymentService> logger, IFusionCache cache, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _cache = cache;
        _clientFactory = clientFactory;
    }

    public async Task<Guid> CreateAsync(CreatePaymentRequest request)
    {
        var region = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .FirstOrDefault(culture => new RegionInfo(culture.Name).TwoLetterISORegionName == request.TwoLetterISORegionName);

        var culture = region is null
            ? CultureInfo.InvariantCulture
            : new CultureInfo(region.Name);

        var paymentId = Guid.NewGuid();

        var payment = new Payment
        {
            Id = paymentId,
            MerchantName = request.MerchantName,
            Items = request.Items.Select(item => new Item
            {
                Name = item.Name,
                SubText = item.SubText,
                TotalPrice = item.TotalPrice,
            }),
            TotalPrice = request.TotalPrice,
            Culture = culture,
            CurrencyCode = new RegionInfo(culture.Name).ISOCurrencySymbol,
            CallbackUrl = request.CallbackUrl,
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl
        };

        await _cache.SetAsync(paymentId.ToString(), payment, TimeSpan.FromMinutes(15));

        return paymentId;
    }

    public async Task<Payment?> GetAsync(Guid paymentId)
    {
        return await _cache.GetOrDefaultAsync<Payment>(paymentId.ToString());
    }

    public async Task<string?> ProcessPaymentAsync(Guid paymentId, bool isCancelled = false)
    {
        var payment = await _cache.GetOrDefaultAsync<Payment>(paymentId.ToString());

        if (payment is null)
        {
            return $"Payment with id '{paymentId}' not found.";
        }

        var client = _clientFactory.CreateClient();

        var result = await client.PostAsJsonAsync(payment.CallbackUrl, new PaymentResult
        {
            PaymentId = paymentId,
            IsSuccessful = !isCancelled,
            Message = isCancelled ? "Cancelled" : "Success"
        });

        _logger.LogInformation("Callback result: {StatusCode}", result.StatusCode);

        await _cache.RemoveAsync(paymentId.ToString());

        return null;
    }
}
