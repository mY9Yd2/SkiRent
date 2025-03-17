using System.IO.Abstractions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using AutoFixture;

using Microsoft.Extensions.Options;

using Refit;

using SkiRent.Api.Configurations;
using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;

using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.IntegrationTests.Systems.Controllers.Invoices
{
    public class TestGet
    {
        private SkiRentWebApplicationFactory<Program> _factory;
        private ISkiRentApi _client;
        private Fixture _fixture;
        private IUserService _userService;
        private PaymentGatewayOptions _paymentGatewayOptions;
        private AppSettings _appSettings;
        private IFileSystem _fileSystem;

        [SetUp]
        public void Setup()
        {
            _factory = new SkiRentWebApplicationFactory<Program>();
            _client = RestService.For<ISkiRentApi>(_factory.CreateClient());

            _userService = _factory.GetRequiredService<IUserService>();
            _paymentGatewayOptions = _factory.GetRequiredService<IOptions<PaymentGatewayOptions>>().Value;
            _appSettings = _factory.GetRequiredService<IOptions<AppSettings>>().Value;
            _fileSystem = _factory.GetRequiredService<IFileSystem>();

            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
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

            await _client.Auth.SignInAsync(user.SignInRequest);

            // Act
            var response = await _client.Invoices.GetAsync(paymentDetails.PaymentId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsInvoiceFile()
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

            var path = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Invoices");
            var directory = _fileSystem.Directory.CreateDirectory(path);
            var fileName = $"{paymentResult.PaymentId}.pdf";
            var filePath = _fileSystem.Path.Combine(directory.FullName, fileName);

            var invoiceFile = await _fileSystem.File.ReadAllBytesAsync(filePath);
            Assert.That(invoiceFile, Is.Not.Null);

            var expectedFileName = $"Számla_{paymentDetails.PaymentId}.pdf";

            await _client.Auth.SignInAsync(user.SignInRequest);

            // Act
            var response = await _client.Invoices.GetAsync(paymentDetails.PaymentId);
            var content = response.Content;
            var invoiceFileName = response.ContentHeaders?.ContentDisposition?.FileNameStar;

            // Assert
            Assert.That(content, Is.Not.Null);

            var contentBytes = await content.ReadAsByteArrayAsync();

            Assert.That(contentBytes, Is.EqualTo(invoiceFile));

            Assert.That(invoiceFileName, Is.Not.Null);
            Assert.That(invoiceFileName, Is.EqualTo(expectedFileName));
        }
    }
}
