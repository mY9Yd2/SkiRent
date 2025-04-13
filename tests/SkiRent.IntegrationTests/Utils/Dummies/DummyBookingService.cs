using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Common;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.IntegrationTests.Utils.Dummies
{
    public class DummyBookingService : BookingService
    {
        public DummyBookingService(
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> appSettings,
            IOptions<PaymentGatewayOptions> paymentGatewayOptions,
            IHttpClientFactory clientFactory,
            [FromKeyedServices("SkiRent.Api.Cache")] IFusionCache cache,
            TimeProvider timeProvider)
                : base(unitOfWork, appSettings, paymentGatewayOptions, clientFactory, cache, timeProvider)
        { }

        protected override Task<Guid> CreatePaymentAsync(IEnumerable<PaymentItem> paymentItems, decimal totalPrice, Uri successUrl, Uri cancelUrl)
        {
            var result = Guid.Parse("4eefc925-28da-492d-9a6a-6c95f5934135");
            return Task.FromResult(result);
        }
    }
}
