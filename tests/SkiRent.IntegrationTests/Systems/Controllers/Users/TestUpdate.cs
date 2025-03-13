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
            var createdAdminUser = (await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin)).Value;
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var request = new UpdateUserRequest
            {
                Email = "new@example.com",
                Password = "NewPassword123",
                Role = RoleTypes.Customer,
                CurrentPassword = string.Empty
            };

            // Act
            var response = await _client.Users.UpdateAsync(createdAdminUser.Id, request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUpdatedUser()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            var createdAdminUser = (await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin)).Value;
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var request = new UpdateUserRequest
            {
                Email = "new@example.com",
                Password = "NewPassword123",
                Role = RoleTypes.Customer,
                CurrentPassword = string.Empty
            };

            var expectedResponse = new GetUserResponse
            {
                Id = createdAdminUser.Id,
                Email = request.Email,
                Role = request.Role.Value
            };

            // Act
            var response = await _client.Users.UpdateAsync(createdAdminUser.Id, request);

            // Assert
            Assert.That(response.Content, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task CanSignInWithUpdatedCredentials_ReturnsOKStatus()
        {
            // Arrange
            var admin = TestDataHelper.CreateUser(_fixture);
            var createdAdminUser = (await _userService.CreateAsync(admin.CreateUserRequest, RoleTypes.Admin)).Value;
            await _client.Auth.SignInAsync(admin.SignInRequest);

            var updateRequest = new UpdateUserRequest
            {
                Email = "new@example.com",
                Password = "NewPassword123",
                Role = RoleTypes.Customer,
                CurrentPassword = string.Empty
            };
            await _client.Users.UpdateAsync(createdAdminUser.Id, updateRequest);

            admin.SignInRequest = admin.SignInRequest with
            {
                Email = updateRequest.Email,
                Password = updateRequest.Password
            };

            // Act
            var response = await _client.Auth.SignInAsync(admin.SignInRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
