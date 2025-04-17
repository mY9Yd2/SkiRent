using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.UnitTests.Systems.Api.Services.Bookings;

public class TestIsValidStatusTransition
{
    private IBookingService _bookingService;

    private IOptions<AppSettings> _appSettings;
    private IOptions<PaymentGatewayOptions> _paymentGatewayOptions;

    [SetUp]
    public void Setup()
    {
        _appSettings = Substitute.For<IOptions<AppSettings>>();
        _paymentGatewayOptions = Substitute.For<IOptions<PaymentGatewayOptions>>();

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
            null!
        );
    }

    private bool InvokeIsValidStatusTransition(string currentStatus, BookingStatusTypes? newStatus)
    {
        var method = typeof(BookingService).GetMethod("IsValidStatusTransition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.That(method, Is.Not.Null);
        return (bool)method.Invoke(_bookingService, [currentStatus, newStatus])!;
    }

    [TestCase(BookingStatus.Paid, BookingStatusTypes.InDelivery, true)]
    [TestCase(BookingStatus.Paid, BookingStatusTypes.Received, true)]
    [TestCase(BookingStatus.InDelivery, BookingStatusTypes.Received, true)]
    [TestCase(BookingStatus.Received, BookingStatusTypes.Returned, true)]
    [TestCase(BookingStatus.Paid, BookingStatusTypes.Returned, false)]
    [TestCase(BookingStatus.InDelivery, BookingStatusTypes.Returned, false)]
    [TestCase(BookingStatus.Returned, BookingStatusTypes.Paid, false)]
    [TestCase(BookingStatus.Pending, BookingStatusTypes.InDelivery, false)]
    [TestCase(BookingStatus.Received, BookingStatusTypes.Cancelled, false)]
    [TestCase(BookingStatus.Cancelled, BookingStatusTypes.Returned, false)]
    [TestCase("Unknown_random_string", BookingStatusTypes.Received, false)]
    public void IsValidStatusTransition_ReturnsExpected(string currentStatus, BookingStatusTypes newStatus, bool expected)
    {
        var result = InvokeIsValidStatusTransition(currentStatus, newStatus);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsValidStatusTransition_WithNullNewStatus_ReturnsFalse()
    {
        var result = InvokeIsValidStatusTransition(BookingStatus.Paid, null);
        Assert.That(result, Is.False);
    }
}
