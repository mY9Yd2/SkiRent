using System.Globalization;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using SkiRent.FakePay.Configurations;
using SkiRent.FakePay.Models;
using SkiRent.Shared.Contracts.Payments;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.FakePay.Services.Payments;

public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly ClientOptions _clientOptions;
    private readonly IFusionCache _cache;
    private readonly IHttpClientFactory _clientFactory;
    private readonly TimeProvider _timeProvider;

    public PaymentService(
        ILogger<PaymentService> logger,
        IOptions<ClientOptions> clientOptions,
        [FromKeyedServices("FakePay.Cache")] IFusionCache cache,
        IHttpClientFactory clientFactory,
        TimeProvider timeProvider)
    {
        _logger = logger;
        _clientOptions = clientOptions.Value;
        _cache = cache;
        _clientFactory = clientFactory;
        _timeProvider = timeProvider;
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

        var paymentResult = new PaymentResult
        {
            PaymentId = paymentId,
            IsSuccessful = !isCancelled,
            Message = isCancelled ? "Cancelled" : "Success",
            PaidAt = isCancelled ? null : _timeProvider.GetUtcNow()
        };

        await SendPaymentCallbackAsync(payment.CallbackUrl, paymentResult);

        await _cache.RemoveAsync(paymentId.ToString());

        return null;
    }

    private async Task SendPaymentCallbackAsync(Uri callbackUrl, PaymentResult paymentResult)
    {
        var client = _clientFactory.CreateClient();

        var message = CreateHttpRequestMessage(callbackUrl, paymentResult);
        var result = await client.SendAsync(message);

        _logger.LogInformation("Callback result: {StatusCode}", result.StatusCode);
    }

    private HttpRequestMessage CreateHttpRequestMessage(Uri callbackUrl, PaymentResult paymentResult)
    {
        var jsonBody = JsonSerializer.Serialize(paymentResult);
        var content = new StringContent(jsonBody, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_clientOptions.SharedSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonBody));
        var signature = Convert.ToBase64String(hashBytes);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = callbackUrl,
            Content = content
        };

        request.Headers.Add("X-Signature", signature);

        return request;
    }
}
