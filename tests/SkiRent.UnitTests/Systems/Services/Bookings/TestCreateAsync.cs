using System.Linq.Expressions;

using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Bookings;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.UnitTests.Systems.Services.Bookings;

public class TestCreateAsync
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
    public async Task WhenUserNotFound_ReturnsFailedResult()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var request = _fixture.Create<CreateBookingRequest>();

        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(false);

        // Act
        var result = await _bookingService.CreateAsync(userId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("userId"), Is.EqualTo(userId));
    }

    [Test]
    public async Task WhenEquipmentNotFound_ReturnsFailedResult()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var request = _fixture.Create<CreateBookingRequest>();
        var equipmentId = request.Equipments.First().EquipmentId;

        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(true);
        _unitOfWork.Equipments
            .GetByIdAsync(equipmentId)
            .Returns((Equipment?)null);

        // Act
        var result = await _bookingService.CreateAsync(userId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("equipmentId"), Is.EqualTo(equipmentId));
    }

    [Test]
    public async Task WhenInsufficientQuantityError_ReturnsFailedResult()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var request = _fixture.Create<CreateBookingRequest>();
        var equipmentRequest = request.Equipments.First();

        var equipment = _fixture.Build<Equipment>()
            .With(equipment => equipment.Id, equipmentRequest.EquipmentId)
            .With(equipment => equipment.AvailableQuantity, equipmentRequest.Quantity - 1)
            .Create();

        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(true);

        _unitOfWork.Equipments
            .GetByIdAsync(equipmentRequest.EquipmentId)
            .Returns(equipment);

        // Act
        var result = await _bookingService.CreateAsync(userId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<InsufficientQuantityError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("equipmentId"), Is.EqualTo(equipmentRequest.EquipmentId));
    }
}
