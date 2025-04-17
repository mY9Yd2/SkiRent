using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.UnitTests.Systems.Api.Services.Bookings;

public class TestCreatePaymentAsync
{
    private Fixture _fixture;
    private IBookingService _bookingService;
    private IOptions<AppSettings> _appSettings;
    private IOptions<PaymentGatewayOptions> _paymentGatewayOptions;
    private IHttpClientFactory _clientFactory;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _appSettings = Substitute.For<IOptions<AppSettings>>();
        _paymentGatewayOptions = Substitute.For<IOptions<PaymentGatewayOptions>>();
        _clientFactory = Substitute.For<IHttpClientFactory>();

        _appSettings.Value.Returns(new AppSettings
        {
            BaseUrl = new Uri("http://localhost"),
            DataDirectoryPath = string.Empty,
            MerchantName = string.Empty,
        });

        _paymentGatewayOptions.Value.Returns(new PaymentGatewayOptions
        {
            BaseUrl = new Uri("http://localhost"),
            SharedSecret = string.Empty,
        });

        _bookingService = new BookingService(
            null!,
            _appSettings,
            _paymentGatewayOptions,
            _clientFactory,
            null!,
            null!
        );
    }

    private Task<Guid> InvokeCreatePaymentAsync(IEnumerable<PaymentItem> paymentItems, decimal totalPrice, Uri successUrl, Uri cancelUrl)
    {
        var method = typeof(BookingService).GetMethod("CreatePaymentAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.That(method, Is.Not.Null);
        return (Task<Guid>)method.Invoke(_bookingService, [paymentItems, totalPrice, successUrl, cancelUrl])!;
    }

    [Test]
    public async Task CreatePaymentAsync_ReturnsExpectedGuid()
    {
        // Arrange
        var paymentItems = _fixture.CreateMany<PaymentItem>(1);
        var totalPrice = paymentItems.Sum(item => item.TotalPrice);
        var successUrl = _fixture.Create<Uri>();
        var cancelUrl = _fixture.Create<Uri>();
        var expectedGuid = _fixture.Create<Guid>();

        var mockHandler = new SuccessfulPaymentResponseHandler(expectedGuid);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = _paymentGatewayOptions.Value.BaseUrl
        };

        _clientFactory.CreateClient()
            .Returns(httpClient);

        // Act
        var result = await InvokeCreatePaymentAsync(paymentItems, totalPrice, successUrl, cancelUrl);

        // Assert
        Assert.That(result, Is.EqualTo(expectedGuid));
    }

    [Test]
    public void Throws_WhenHttpResponseIsNotSuccessful()
    {
        // Arrange
        var paymentItems = _fixture.CreateMany<PaymentItem>(1);
        var totalPrice = paymentItems.Sum(item => item.TotalPrice);
        var successUrl = _fixture.Create<Uri>();
        var cancelUrl = _fixture.Create<Uri>();

        var mockHandler = new FailureResponseHandler();
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = _paymentGatewayOptions.Value.BaseUrl
        };

        _clientFactory.CreateClient()
            .Returns(httpClient);

        // Act & Assert
        var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
            await InvokeCreatePaymentAsync(paymentItems, totalPrice, successUrl, cancelUrl));

        Assert.That(exception.Message, Is.EqualTo("Response status code does not indicate success: 500 (Internal Server Error)."));
    }

    [Test]
    public async Task PostsCorrectRequest_AndReturnsExpectedGuid()
    {
        // Arrange
        var paymentItems = _fixture.CreateMany<PaymentItem>(1).ToList();
        var totalPrice = paymentItems.Sum(item => item.TotalPrice);
        var successUrl = _fixture.Create<Uri>();
        var cancelUrl = _fixture.Create<Uri>();
        var expectedGuid = _fixture.Create<Guid>();

        CreatePaymentRequest? capturedRequest = null;

        var mockHandler = new RequestVerificationHandler(request =>
        {
            Assert.That(request.RequestUri?.AbsolutePath, Is.EqualTo("/api/payments"));
            Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));

            var json = request.Content!.ReadAsStringAsync().Result;
            capturedRequest = JsonSerializer.Deserialize<CreatePaymentRequest>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expectedGuid)
            };
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = _paymentGatewayOptions.Value.BaseUrl
        };

        _clientFactory.CreateClient()
            .Returns(httpClient);

        // Act
        var result = await InvokeCreatePaymentAsync(paymentItems, totalPrice, successUrl, cancelUrl);

        // Assert
        Assert.That(result, Is.EqualTo(expectedGuid));
        Assert.That(capturedRequest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(capturedRequest.TwoLetterISORegionName, Is.EqualTo("HU"));
            Assert.That(capturedRequest.Items.Count(), Is.EqualTo(paymentItems.Count));
            Assert.That(capturedRequest.TotalPrice, Is.EqualTo(totalPrice));
            Assert.That(capturedRequest.SuccessUrl, Is.EqualTo(successUrl));
            Assert.That(capturedRequest.CancelUrl, Is.EqualTo(cancelUrl));
            Assert.That(capturedRequest.CallbackUrl, Is.Not.Null);
        });
    }

    private class SuccessfulPaymentResponseHandler : HttpMessageHandler
    {
        private readonly Guid _responseGuid;

        public SuccessfulPaymentResponseHandler(Guid responseGuid)
        {
            _responseGuid = responseGuid;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(_responseGuid)
            };
            return Task.FromResult(response);
        }
    }

    private class FailureResponseHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            return Task.FromResult(response);
        }
    }

    private class RequestVerificationHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _verifyAndRespond;

        public RequestVerificationHandler(Func<HttpRequestMessage, HttpResponseMessage> verifyAndRespond)
        {
            _verifyAndRespond = verifyAndRespond;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_verifyAndRespond(request));
        }
    }
}
