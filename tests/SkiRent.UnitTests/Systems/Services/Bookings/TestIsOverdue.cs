using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.UnitTests.Systems.Services.Bookings;

public class TestIsOverdue
{
    private IBookingService _bookingService;

    private IOptions<AppSettings> _appSettings;
    private IOptions<PaymentGatewayOptions> _paymentGatewayOptions;
    private TimeProvider _timeProvider;

    [SetUp]
    public void Setup()
    {
        _appSettings = Substitute.For<IOptions<AppSettings>>();
        _paymentGatewayOptions = Substitute.For<IOptions<PaymentGatewayOptions>>();
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
            null!,
            _appSettings,
            _paymentGatewayOptions,
            null!,
            null!,
            _timeProvider
        );
    }

    private bool InvokeIsOverdue(DateOnly endDate, string status)
    {
        var method = typeof(BookingService).GetMethod("IsOverdue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.That(method, Is.Not.Null);
        return (bool)method.Invoke(_bookingService, [endDate, status])!;
    }

    [Test]
    public void WhenBookingIsPaidAndEndDateIsInPast_ReturnsTrue()
    {
        // Arrange
        var utcNow = DateTimeOffset.UtcNow;

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        var endDate = new DateOnly(utcNow.Year, utcNow.Month, utcNow.Day - 1);
        var status = BookingStatusTypes.Paid.ToString();

        // Act
        var result = InvokeIsOverdue(endDate, status);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void WhenBookingIsNotPaid_ReturnsFalse()
    {
        // Arrange
        var utcNow = DateTimeOffset.UtcNow;

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        var endDate = new DateOnly(utcNow.Year, utcNow.Month, utcNow.Day - 1);
        var status = BookingStatusTypes.Pending.ToString();

        // Act
        var result = InvokeIsOverdue(endDate, status);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void WhenEndDateIsTodayOrFuture_ReturnsFalse()
    {
        // Arrange
        var utcNow = DateTimeOffset.UtcNow;

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        var endDate = new DateOnly(utcNow.Year, utcNow.Month, utcNow.Day);
        var status = BookingStatusTypes.Paid.ToString();

        // Act
        var result = InvokeIsOverdue(endDate, status);

        // Assert
        Assert.That(result, Is.False);
    }
}
