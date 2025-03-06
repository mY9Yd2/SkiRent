using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Bookings;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.IntegrationTests.Systems.Controllers.Bookings
{
    public class TestGetAll
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
            var createdBookingResponse = (await _client.Bookings.CreateAsync(createBookingRequest)).Content;
            Assert.That(createdBookingResponse, Is.Not.Null);

            // Act
            var response = await _client.Bookings.GetAllAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsListOfBookings()
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
            var createdBookingResponse = (await _client.Bookings.CreateAsync(createBookingRequest)).Content;
            Assert.That(createdBookingResponse, Is.Not.Null);

            var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(createdBookingResponse.Id);
            Assert.That(booking, Is.Not.Null);

            var expectedResponse = new List<GetAllBookingResponse>()
            {
                new()
                {
                    Id = booking.Id,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    TotalPrice = booking.TotalPrice,
                    PaymentId = booking.PaymentId,
                    Status = Enum.Parse<BookingStatusTypes>(booking.Status),
                    CreatedAt = booking.CreatedAt
                }
            };

            // Act
            var response = await _client.Bookings.GetAllAsync();
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
