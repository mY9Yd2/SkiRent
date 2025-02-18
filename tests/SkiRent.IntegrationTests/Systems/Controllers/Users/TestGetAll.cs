using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Systems.Controllers.Users
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

            // Act
            var response = await _client.Users.GetAllAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsListOfUsers()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            var createdAdminUser = (await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin)).Value;

            var user = TestDataHelper.CreateUser(_fixture);
            var createdUser = (await _userService.CreateAsync(user.CreateUserRequest)).Value;

            await _client.Auth.SignInAsync(admin.SignInRequest);

            var expectedResponse = new List<GetAllUserResponse>
            {
                new()
                {
                    Id = createdAdminUser.Id,
                    Email = createdAdminUser.Email,
                    Role = createdAdminUser.Role
                },
                new()
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Role = createdUser.Role
                }
            };

            // Act
            var response = await _client.Users.GetAllAsync();

            // Assert
            Assert.That(response.Content, Is.EqualTo(expectedResponse));
        }
    }
}
