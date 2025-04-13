using System.IO.Abstractions;

using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Services.Payments;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.Shared.Contracts.Payments;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.UnitTests.Systems.Services.Payments;

public class TestProcessPaymentCallbackAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IPaymentService _paymentService;
    private IOptions<AppSettings> _appSettings;
    private IFusionCache _cache;
    private IFileSystem _fileSystem;
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
        _cache = Substitute.For<IFusionCache>();
        _fileSystem = Substitute.For<IFileSystem>();
        _timeProvider = Substitute.For<TimeProvider>();

        _appSettings.Value.Returns(new AppSettings
        {
            BaseUrl = new Uri("http://localhost"),
            DataDirectoryPath = string.Empty,
            MerchantName = string.Empty,
        });

        _paymentService = new PaymentService(_unitOfWork, _cache, _appSettings, _fileSystem, _timeProvider);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
        _cache.Dispose();
    }

    [Test]
    public void WhenBookingNotFound_ThrowsPaymentNotFoundException()
    {
        // Arrange
        var paymentResult = _fixture.Create<PaymentResult>();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns((Booking?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<PaymentNotFoundException>(() =>
            _paymentService.ProcessPaymentCallbackAsync(paymentResult));

        Assert.That(exception.Message, Is.EqualTo($"No booking found for Payment ID: {paymentResult.PaymentId}"));
    }

    [Test]
    public void WhenEquipmentNotFound_ThrowsBookingRollbackException()
    {
        // Arrange
        var paymentResult = _fixture.Build<PaymentResult>()
            .With(result => result.IsSuccessful, false)
            .Create();
        var bookingItem = _fixture.Create<BookingItem>();

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.PaymentId, paymentResult.PaymentId)
            .With(booking => booking.BookingItems, [bookingItem])
            .Create();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns(booking);

        _unitOfWork.Equipments
            .GetByIdAsync(bookingItem.EquipmentId)
            .Returns((Equipment?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<BookingRollbackException>(() =>
            _paymentService.ProcessPaymentCallbackAsync(paymentResult));

        Assert.That(exception.Message, Is.EqualTo($"Equipment with id '{bookingItem.EquipmentId}' not found."));
    }

    [Test]
    public async Task WhenPaymentIsSuccessful_SavesChangesAndCreatesInvoice()
    {
        // Arrange
        var paymentResult = _fixture.Build<PaymentResult>()
            .With(result => result.IsSuccessful, true)
            .Create();

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.PaymentId, paymentResult.PaymentId)
            .With(booking => booking.Status, BookingStatus.Pending)
            .Create();

        var invoiceRequest = _fixture.Create<CreateInvoiceRequest>();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns(booking);

        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        _cache.GetOrDefaultAsync<CreateInvoiceRequest>(paymentResult.PaymentId.ToString())
            .Returns(invoiceRequest);

        // Act
        var result = await _paymentService.ProcessPaymentCallbackAsync(paymentResult);

        // Assert
        await _unitOfWork.Received(2).SaveChangesAsync();
        await _unitOfWork.Invoices.Received(1).AddAsync(
            Arg.Is<Invoice>(invoice =>
                invoice.Id == paymentResult.PaymentId &&
                invoice.UserId == booking.UserId &&
                invoice.BookingId == booking.Id
            ));
        await _cache.Received(1).RemoveAsync(paymentResult.PaymentId.ToString());
        Assert.That(booking.Status, Is.EqualTo(BookingStatus.Paid));
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void WhenCreateInvoiceRequestNotFound_ThrowsCreateInvoiceRequestNotFoundException()
    {
        // Arrange
        var paymentResult = _fixture.Build<PaymentResult>()
            .With(result => result.IsSuccessful, true)
            .Create();

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.PaymentId, paymentResult.PaymentId)
            .With(booking => booking.Status, BookingStatus.Pending)
            .Create();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns(booking);

        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = Assert.ThrowsAsync<CreateInvoiceRequestNotFoundException>(() =>
            _paymentService.ProcessPaymentCallbackAsync(paymentResult));

        Assert.That(exception.Message, Is.EqualTo($"Invoice with payment id '{paymentResult.PaymentId}' not found."));
    }

    [Test]
    public async Task WhenPaymentIsUnsuccessful_EquipmentQuantityRemainsUnchanged()
    {
        // Arrange
        var paymentResult = _fixture.Build<PaymentResult>()
            .With(result => result.IsSuccessful, false)
            .Create();
        var bookingItem = _fixture.Create<BookingItem>();

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.PaymentId, paymentResult.PaymentId)
            .With(booking => booking.Status, BookingStatus.Pending)
            .With(booking => booking.BookingItems, [bookingItem])
            .Create();

        var invoiceRequest = _fixture.Create<CreateInvoiceRequest>();
        var equipment = _fixture.Create<Equipment>();

        var expectedAvailableQuantity = equipment.AvailableQuantity + bookingItem.Quantity;

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns(booking);

        _unitOfWork.Equipments
            .GetByIdAsync(bookingItem.EquipmentId)
            .Returns(equipment);

        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        _cache.GetOrDefaultAsync<CreateInvoiceRequest>(paymentResult.PaymentId.ToString())
            .Returns(invoiceRequest);

        // Act
        var result = await _paymentService.ProcessPaymentCallbackAsync(paymentResult);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(equipment.AvailableQuantity, Is.EqualTo(expectedAvailableQuantity));
    }

    [TestCase(true, BookingStatus.Paid)]
    [TestCase(false, BookingStatus.Cancelled)]
    public async Task BookingStatus_IsSetCorrectly_BasedOnPaymentSuccess(bool isSuccessful, string expectedStatus)
    {
        // Arrange
        var paymentResult = _fixture.Build<PaymentResult>()
            .With(result => result.IsSuccessful, isSuccessful)
            .Create();

        var booking = _fixture.Build<Booking>()
            .With(booking => booking.PaymentId, paymentResult.PaymentId)
            .With(booking => booking.BookingItems, [])
            .Create();

        _unitOfWork.Bookings
            .GetBookingWithItemsAsync(paymentResult.PaymentId)
            .Returns(booking);

        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        if (isSuccessful)
        {
            var invoiceRequest = _fixture.Create<CreateInvoiceRequest>();
            _cache.GetOrDefaultAsync<CreateInvoiceRequest>(paymentResult.PaymentId.ToString())
                .Returns(invoiceRequest);
        }

        // Act
        await _paymentService.ProcessPaymentCallbackAsync(paymentResult);

        // Assert
        Assert.That(booking.Status, Is.EqualTo(expectedStatus));
    }
}
