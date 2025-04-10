using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Extensions;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.UnitTests.Systems.Services.Bookings;

public class TestUpdateAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IBookingService _bookingService;

    private IOptions<AppSettings> _appSettings;
    private IOptions<PaymentGatewayOptions> _paymentGatewayOptions;
    private IHttpClientFactory _clientFactory;
    private IFusionCache _cache;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _appSettings = Substitute.For<IOptions<AppSettings>>();
        _paymentGatewayOptions = Substitute.For<IOptions<PaymentGatewayOptions>>();
        _clientFactory = Substitute.For<IHttpClientFactory>();
        _cache = Substitute.For<IFusionCache>();

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
            _unitOfWork,
            _appSettings,
            _paymentGatewayOptions,
            _clientFactory,
            _cache
        );
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
        _cache.Dispose();
    }

    [Test]
    public async Task WhenBookingNotFound_ReturnsFailedResult()
    {
        // Arrange
        var bookingId = _fixture.Create<int>();
        var request = _fixture.Create<UpdateBookingRequest>();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(bookingId)
            .Returns((Booking?)null);

        // Act
        var result = await _bookingService.UpdateAsync(bookingId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<BookingNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("bookingId"), Is.EqualTo(bookingId));
    }

    [Test]
    public async Task WhenInvalidStatusTransition_ReturnsFailedResult()
    {
        // Arrange
        var bookingId = _fixture.Create<int>();
        var request = new UpdateBookingRequest { Status = BookingStatusTypes.Cancelled };

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.Id, bookingId)
            .With(booking => booking.Status, BookingStatus.Returned)
            .Create();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(bookingId)
            .Returns(booking);

        // Act
        var result = await _bookingService.UpdateAsync(bookingId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<InvalidBookingStatusTransitionError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("currentBookingStatus"), Is.EqualTo(booking.Status));
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("requestedBookingStatus"), Is.EqualTo(((BookingStatusTypes)request.Status).ToBookingStatusString()));
    }
}
