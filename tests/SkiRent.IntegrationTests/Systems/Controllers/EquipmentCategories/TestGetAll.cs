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

            var request = TestDataHelper.CreateEquipmentCategory(_fixture);
            await _client.EquipmentCategories.CreateAsync(request);

            // Act
            var response = await _client.EquipmentCategories.GetAllAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsListOfEquipmentCategories()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin);
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var createRequests = _fixture.Build<CreateEquipmentCategoryRequest>()
                .With(request => request.Name)
                .CreateMany(2);

            var createdCategories = new List<CreatedEquipmentCategoryResponse>();
            foreach (var createRequest in createRequests)
            {
                var createdCategory = (await _client.EquipmentCategories.CreateAsync(createRequest)).Content;
                Assert.That(createdCategory, Is.Not.Null);
                createdCategories.Add(createdCategory);
            }

            var expectedResponse = new List<GetAllEquipmentCategoryResponse>
            {
                new()
                {
                    Id = createdCategories[0].Id,
                    Name = createdCategories[0].Name
                },
                new()
                {
                    Id = createdCategories[1].Id,
                    Name = createdCategories[1].Name
                }
            };

            // Act
            var response = await _client.EquipmentCategories.GetAllAsync();
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
