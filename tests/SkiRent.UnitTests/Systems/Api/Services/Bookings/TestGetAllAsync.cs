using System.Linq.Expressions;

using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Services.Bookings;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.UnitTests.Systems.Api.Services.Bookings;

public class TestGetAllAsync
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
    public async Task WhenUserIsAdmin_ReturnsAllBookings()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var bookings = _fixture.Build<Booking>()
            .With(booking => booking.Status, BookingStatus.Paid)
            .CreateMany(3)
            .ToList();
        Func<string, bool> isInRole = _ => true;

        _unitOfWork.Bookings
            .GetAllAsync()
            .Returns(bookings);

        // Act
        var result = await _bookingService.GetAllAsync(userId, isInRole);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Exactly(bookings.Count).Items);
        await _unitOfWork.Bookings.Received(1).GetAllAsync();
        await _unitOfWork.Bookings.DidNotReceive().FindAllAsync(Arg.Any<Expression<Func<Booking, bool>>>());
    }

    [Test]
    public async Task WhenUserIsNotAdmin_ReturnsOwnBookingsOnly()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var bookings = _fixture.Build<Booking>()
            .With(booking => booking.UserId, userId)
            .With(booking => booking.Status, BookingStatus.Paid)
            .CreateMany(2)
            .ToList();
        Func<string, bool> isInRole = _ => false;

        _unitOfWork.Bookings
            .FindAllAsync(Arg.Any<Expression<Func<Booking, bool>>>())
            .Returns(bookings);

        // Act
        var result = await _bookingService.GetAllAsync(userId, isInRole);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Exactly(bookings.Count).Items);
        await _unitOfWork.Bookings.Received(1).FindAllAsync(Arg.Any<Expression<Func<Booking, bool>>>());
        await _unitOfWork.Bookings.DidNotReceive().GetAllAsync();
    }
}
