using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Bookings;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.UnitTests.Systems.Api.Services.Bookings;

public class TestDeleteAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IBookingService _bookingService;

    private IOptions<AppSettings> _appSettings;
    private IOptions<PaymentGatewayOptions> _paymentGatewayOptions;
    private IHttpClientFactory _clientFactory;
    private IFusionCache _cache;
    private TimeProvider _timeProvider;

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
        _timeProvider = Substitute.For<TimeProvider>();

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
            _cache,
            _timeProvider
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

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(bookingId)
            .Returns((Booking?)null);

        // Act
        var result = await _bookingService.DeleteAsync(bookingId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<BookingNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("bookingId"), Is.EqualTo(bookingId));
    }

    [Test]
    public async Task WhenBookingIsNotCancelledOrReturned_ReturnsFailedResult()
    {
        // Arrange
        var bookingId = _fixture.Create<int>();
        var booking = _fixture.Build<Booking>()
            .With(booking => booking.Id, bookingId)
            .With(booking => booking.Status, BookingStatus.Paid)
            .Create();

        _unitOfWork.Bookings
            .GetByIdAsync(bookingId)
            .Returns(booking);

        // Act
        var result = await _bookingService.DeleteAsync(bookingId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<BookingDeletionNotAllowedError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("bookingId"), Is.EqualTo(bookingId));
    }

    [Test]
    public async Task WhenBookingExists_DeletesBookingAndSavesChanges()
    {
        // Arrange
        var booking = _fixture.Build<Booking>()
            .With(booking => booking.Status, BookingStatus.Cancelled)
            .Create();

        _unitOfWork.Bookings
            .GetByIdAsync(booking.Id)
            .Returns(booking);

        // Act
        var result = await _bookingService.DeleteAsync(booking.Id);

        // Assert
        Assert.That(result.IsFailed, Is.False);
        _unitOfWork.Bookings.Received(1).Delete(booking);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
