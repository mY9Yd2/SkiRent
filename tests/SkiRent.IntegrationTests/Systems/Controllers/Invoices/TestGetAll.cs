using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using AutoFixture;

using Microsoft.Extensions.Options;

using Refit;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;

using SkiRent.Shared.Clients;

using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.IntegrationTests.Systems.Controllers.Invoices;

public class TestGetAll
{
    private SkiRentWebApplicationFactory<Program> _factory;
    private ISkiRentApi _client;
    private Fixture _fixture;
    private IUserService _userService;
    private PaymentGatewayOptions _paymentGatewayOptions;
    private IUnitOfWork _unitOfWork;

    [SetUp]
    public void Setup()
    {
        _factory = new SkiRentWebApplicationFactory<Program>();
        _client = RestService.For<ISkiRentApi>(_factory.CreateClient());

        _userService = _factory.GetRequiredService<IUserService>();
        _paymentGatewayOptions = _factory.GetRequiredService<IOptions<PaymentGatewayOptions>>().Value;
        _unitOfWork = _factory.GetRequiredService<IUnitOfWork>();

        _fixture = new Fixture();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
        _unitOfWork.Dispose();
    }

    private string CreateSignature(PaymentResult paymentResult)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonBody = JsonSerializer.Serialize(paymentResult, options);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_paymentGatewayOptions.SharedSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonBody));
        var signature = Convert.ToBase64String(hashBytes);

        return signature;
    }

    [Test]
    public async Task WhenSuccessful_ReturnsOKStatus()
    {
        // Arrange
        var admin = TestDataHelper.CreateUser(_fixture);
        await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
        await _client.Auth.SignInAsync(admin.SignInRequest);

        // Act
        var response = await _client.Invoices.GetAllAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task WhenSuccessful_ReturnsListOfInvoices()
    {
        // Arrange
        var admin = TestDataHelper.CreateUser(_fixture);
        await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
        await _client.Auth.SignInAsync(admin.SignInRequest);

        var createCategoryRequest = TestDataHelper.CreateEquipmentCategory(_fixture);
        var category = (await _client.EquipmentCategories.CreateAsync(createCategoryRequest)).Content;
        Assert.That(category, Is.Not.Null);

        var createEquipmentRequest = TestDataHelper.CreateEquipment(_fixture);
        var equipment = (await _client.Equipments.CreateAsync(createEquipmentRequest)).Content;
        Assert.That(equipment, Is.Not.Null);

        var user = TestDataHelper.CreateUser(_fixture);
        await _client.Users.CreateAsync(user.CreateUserRequest);
        await _client.Auth.SignInAsync(user.SignInRequest);

        var equipmentBookings = new List<EquipmentBooking>()
        {
            new() { EquipmentId = equipment.Id, Quantity = 1 }
        };

        var createBookingRequest = TestDataHelper.CreateBooking(_fixture, equipmentBookings);
        var paymentDetails = (await _client.Bookings.CreateAsync(createBookingRequest)).Content;
        Assert.That(paymentDetails, Is.Not.Null);
        await _client.Auth.SignOutAsync(string.Empty);

        var paymentResult = new PaymentResult
        {
            PaymentId = paymentDetails.PaymentId,
            IsSuccessful = true,
            Message = "Success",
            PaidAt = TimeProvider.System.GetUtcNow(),
        };

        var signature = CreateSignature(paymentResult);
        await _client.Payments.CallbackAsync(signature, paymentResult);

        await _client.Auth.SignInAsync(admin.SignInRequest);

        var invoice = await _unitOfWork.Invoices.GetByIdAsync(paymentDetails.PaymentId);
        Assert.That(invoice, Is.Not.Null);

        var expectedResponse = new List<GetAllInvoicesResponse>
        {
            new()
            {
                Id = Guid.Parse("4eefc925-28da-492d-9a6a-6c95f5934135"),
                UserId = 2,
                BookingId = paymentDetails.Id,
                CreatedAt = invoice.CreatedAt,
                Email = user.CreateUserRequest.Email
            }
        };

        // Act
        var response = await _client.Invoices.GetAllAsync();

        // Assert
        Assert.That(response.Content, Is.EqualTo(expectedResponse));
    }
}
