using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.IntegrationTests.Systems.Controllers.Equipments
{
    public class TestGet
    {
        private SkiRentWebApplicationFactory<Program> _factory;
        private ISkiRentApi _client;
        private Fixture _fixture;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _factory = new SkiRentWebApplicationFactory<Program>();
            _client = RestService.For<ISkiRentApi>(_factory.CreateClient());

            _userService = _factory.GetRequiredService<IUserService>();

            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
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
            var createdEquipment = (await _client.Equipments.CreateAsync(createEquipmentRequest)).Content;
            Assert.That(createdEquipment, Is.Not.Null);

            // Act
            var response = await _client.Equipments.GetAsync(createdEquipment.Id);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsEquipment()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var createCategoryRequest = TestDataHelper.CreateEquipmentCategory(_fixture);
            var category = (await _client.EquipmentCategories.CreateAsync(createCategoryRequest)).Content;
            Assert.That(category, Is.Not.Null);

            var createEquipmentRequest = TestDataHelper.CreateEquipment(_fixture);
            var createdEquipment = (await _client.Equipments.CreateAsync(createEquipmentRequest)).Content;
            Assert.That(createdEquipment, Is.Not.Null);

            var expectedResponse = new GetEquipmentResponse
            {
                Id = createdEquipment.Id,
                Name = createdEquipment.Name,
                Description = createdEquipment.Description,
                CategoryId = createdEquipment.CategoryId,
                CategoryName = category.Name,
                PricePerDay = createdEquipment.PricePerDay,
                AvailableQuantity = createdEquipment.AvailableQuantity,
                MainImageId = createdEquipment.MainImageId
            };

            // Act
            var response = await _client.Equipments.GetAsync(createdEquipment.Id);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
