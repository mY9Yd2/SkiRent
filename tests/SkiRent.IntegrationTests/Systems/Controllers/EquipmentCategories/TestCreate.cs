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
    public class TestCreate
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
        public async Task WhenSuccessful_ReturnsCreatedStatus()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var request = new CreateEquipmentCategoryRequest
            {
                Name = _fixture.Create<string>()
            };

            // Act
            var response = await _client.EquipmentCategories.CreateAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsEquipmentCategory()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var request = new CreateEquipmentCategoryRequest
            {
                Name = _fixture.Create<string>()
            };

            var expectedResponse = new CreatedEquipmentCategoryResponse
            {
                Id = 1,
                Name = request.Name
            };

            // Act
            var response = await _client.EquipmentCategories.CreateAsync(request);
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
