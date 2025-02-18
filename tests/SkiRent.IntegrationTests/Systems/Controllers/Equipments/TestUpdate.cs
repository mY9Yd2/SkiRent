using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.IntegrationTests.Systems.Controllers.Equipments
{
    public class TestUpdate
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

            var createRequests = TestDataHelper.CreateManyEquipmentCategory(_fixture);

            var createdCategories = new List<CreatedEquipmentCategoryResponse>();
            foreach (var createRequest in createRequests)
            {
                var createdCategory = (await _client.EquipmentCategories.CreateAsync(createRequest)).Content;
                Assert.That(createdCategory, Is.Not.Null);
                createdCategories.Add(createdCategory);
            }

            var createEquipmentRequest = TestDataHelper.CreateEquipment(_fixture, categoryId: createdCategories[0].Id);
            var createdEquipment = (await _client.Equipments.CreateAsync(createEquipmentRequest)).Content;
            Assert.That(createdEquipment, Is.Not.Null);

            var request = new UpdateEquipmentRequest
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>(),
                CategoryId = createdCategories[1].Id,
                PricePerDay = _fixture.Create<decimal>(),
                AvailableQuantity = _fixture.Create<int>()
            };

            // Act
            var response = await _client.Equipments.UpdateAsync(createdEquipment.Id, request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUpdatedEquipment()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var createRequests = TestDataHelper.CreateManyEquipmentCategory(_fixture);

            var createdCategories = new List<CreatedEquipmentCategoryResponse>();
            foreach (var createRequest in createRequests)
            {
                var createdCategory = (await _client.EquipmentCategories.CreateAsync(createRequest)).Content;
                Assert.That(createdCategory, Is.Not.Null);
                createdCategories.Add(createdCategory);
            }

            var createEquipmentRequest = TestDataHelper.CreateEquipment(_fixture, categoryId: createdCategories[0].Id);
            var createdEquipment = (await _client.Equipments.CreateAsync(createEquipmentRequest)).Content;
            Assert.That(createdEquipment, Is.Not.Null);

            var request = new UpdateEquipmentRequest
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>(),
                CategoryId = createdCategories[1].Id,
                PricePerDay = _fixture.Create<decimal>(),
                AvailableQuantity = _fixture.Create<int>()
            };

            var expectedResponse = new GetEquipmentResponse
            {
                Id = createdEquipment.Id,
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryIdAsNonNull,
                PricePerDay = request.PricePerDayAsNonNull,
                AvailableQuantity = request.AvailableQuantityAsNonNull
            };

            // Act
            var response = await _client.Equipments.UpdateAsync(createdEquipment.Id, request);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
