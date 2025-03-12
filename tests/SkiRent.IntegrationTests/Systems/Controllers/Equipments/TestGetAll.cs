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
    public class TestGetAll
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
            var response = await _client.Equipments.GetAllAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsListOfEquipments()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var createCategoryRequest = TestDataHelper.CreateEquipmentCategory(_fixture);
            var category = (await _client.EquipmentCategories.CreateAsync(createCategoryRequest)).Content;
            Assert.That(category, Is.Not.Null);

            var createEquipmentRequests = TestDataHelper.CreateManyEquipment(_fixture);

            var createdEquipments = new List<CreatedEquipmentResponse>();
            foreach (var createRequest in createEquipmentRequests)
            {
                var createdEquipment = (await _client.Equipments.CreateAsync(createRequest)).Content;
                Assert.That(createdEquipment, Is.Not.Null);
                createdEquipments.Add(createdEquipment);
            }

            var expectedResponse = new List<GetAllEquipmentResponse>()
            {
                new()
                {
                    Id = createdEquipments[0].Id,
                    Name = createdEquipments[0].Name,
                    Description = createdEquipments[0].Description,
                    CategoryId = createdEquipments[0].CategoryId,
                    CategoryName = category.Name,
                    PricePerDay = createdEquipments[0].PricePerDay,
                    AvailableQuantity = createdEquipments[0].AvailableQuantity
                },
                new()
                {
                    Id = createdEquipments[1].Id,
                    Name = createdEquipments[1].Name,
                    Description = createdEquipments[1].Description,
                    CategoryId = createdEquipments[1].CategoryId,
                    CategoryName = category.Name,
                    PricePerDay = createdEquipments[1].PricePerDay,
                    AvailableQuantity = createdEquipments[1].AvailableQuantity
                }
            };

            // Act
            var response = await _client.Equipments.GetAllAsync();
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
