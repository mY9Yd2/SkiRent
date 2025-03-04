using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.IntegrationTests.Systems.Controllers.Bookings
{
    public class TestCreate
    {
        private SkiRentWebApplicationFactory<Program> _factory;
        private ISkiRentApi _client;
        private Fixture _fixture;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            _factory = new SkiRentWebApplicationFactory<Program>();
            _client = RestService.For<ISkiRentApi>(_factory.CreateClient());

            _userService = _factory.GetRequiredService<IUserService>();
            _unitOfWork = _factory.GetRequiredService<IUnitOfWork>();

            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task WhenSuccessful_ReturnsCreatedStatus()
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

            var request = TestDataHelper.CreateBooking(_fixture, equipmentBookings);

            // Act
            var response = await _client.Bookings.CreateAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsPaymentDetails()
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

            var request = TestDataHelper.CreateBooking(_fixture, equipmentBookings);

            var expectedResponse = new CreatedBookingResponse
            {
                Id = 1,
                PaymentId = Guid.Parse("4eefc925-28da-492d-9a6a-6c95f5934135"),
                PaymentUrl = new Uri("http://localhost:5106/payments/4eefc925-28da-492d-9a6a-6c95f5934135")
            };

            // Act
            var response = await _client.Bookings.CreateAsync(request);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task WhenSuccessful_ShouldStoreInDatabase()
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

            var request = TestDataHelper.CreateBooking(_fixture, equipmentBookings);

            var days = (request.EndDate.DayNumber - request.StartDate.DayNumber) + 1;
            var expectedBooking = new Booking
            {
                UserId = 2,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = createEquipmentRequest.PricePerDay * days,
                PaymentId = Guid.Parse("4eefc925-28da-492d-9a6a-6c95f5934135"),
                Status = BookingStatus.Pending
            };
            var expectedBookingItem = new BookingItem
            {
                EquipmentId = equipment.Id,
                NameAtBooking = equipment.Name,
                PriceAtBooking = equipment.PricePerDay,
                Quantity = 1
            };

            // Act
            var response = await _client.Bookings.CreateAsync(request);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(content.PaymentId);

            Assert.That(booking, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(booking.UserId, Is.EqualTo(expectedBooking.UserId));
                Assert.That(booking.StartDate, Is.EqualTo(expectedBooking.StartDate));
                Assert.That(booking.EndDate, Is.EqualTo(expectedBooking.EndDate));
                Assert.That(booking.TotalPrice, Is.EqualTo(expectedBooking.TotalPrice));
                Assert.That(booking.PaymentId, Is.EqualTo(expectedBooking.PaymentId));
                Assert.That(booking.Status, Is.EqualTo(expectedBooking.Status));

                Assert.That(booking.BookingItems, Has.Count.EqualTo(1));
            });

            Assert.Multiple(() =>
            {
                var item = booking.BookingItems.First();
                Assert.That(item.EquipmentId, Is.EqualTo(expectedBookingItem.EquipmentId));
                Assert.That(item.Quantity, Is.EqualTo(expectedBookingItem.Quantity));
            });
        }
    }
}
