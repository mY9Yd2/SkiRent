using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.IntegrationTests.Systems.Controllers.EquipmentCategories
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

            var createCategoryRequest = TestDataHelper.CreateEquipmentCategory(_fixture);
            var createdCategory = (await _client.EquipmentCategories.CreateAsync(createCategoryRequest)).Content;
            Assert.That(createdCategory, Is.Not.Null);

            var request = new UpdateEquipmentCategoryRequest
            {
                Name = _fixture.Create<string>()
            };

            // Act
            var response = await _client.EquipmentCategories.UpdateAsync(createdCategory.Id, request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUpdatedEquipmentCategory()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var createCategoryRequest = TestDataHelper.CreateEquipmentCategory(_fixture);
            var createdCategory = (await _client.EquipmentCategories.CreateAsync(createCategoryRequest)).Content;
            Assert.That(createdCategory, Is.Not.Null);

            var request = new UpdateEquipmentCategoryRequest
            {
                Name = _fixture.Create<string>()
            };

            var expectedResponse = new GetEquipmentCategoryResponse
            {
                Id = createdCategory.Id,
                Name = request.Name
            };

            // Act
            var response = await _client.EquipmentCategories.UpdateAsync(createdCategory.Id, request);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
