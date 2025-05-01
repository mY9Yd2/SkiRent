using System.Globalization;
using System.Reflection;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.UnitTests.Systems.Api.Services.Bookings;

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

    [Test, Combinatorial]
    public void ReturnsTrue_OnlyWhenEndDateIsPast_AndStatusIsPaid(
        [Values("2025-04-30", "2025-04-10")] string endDateString,
        [Values] BookingStatusTypes status)
    {
        // Arrange
        var endDate = DateOnly.Parse(endDateString, new CultureInfo("hu-HU"));
        var utcNow = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero);

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        var expected = status == BookingStatusTypes.Paid;

        // Act
        var result = InvokeIsOverdue(endDate, status.ToString());

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test, Combinatorial]
    public void ReturnsFalse_WhenEndDateIsTodayOrFuture_AndStatusIsPaid(
        [Values("2025-05-01", "2025-05-10")] string endDateString,
        [Values(BookingStatusTypes.Paid)] BookingStatusTypes status)
    {
        // Arrange
        var endDate = DateOnly.Parse(endDateString, new CultureInfo("hu-HU"));
        var utcNow = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero);

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        // Act
        var result = InvokeIsOverdue(endDate, status.ToString());

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void UnknownStatus_ThrowsArgumentException()
    {
        // Arrange
        var endDate = new DateOnly(2025, 4, 30);
        var unknownStatus = Guid.NewGuid().ToString();
        var utcNow = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero);

        _timeProvider.GetUtcNow()
            .Returns(utcNow);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            try
            {
                InvokeIsOverdue(endDate, unknownStatus);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException!;
            }
        });
    }
}
