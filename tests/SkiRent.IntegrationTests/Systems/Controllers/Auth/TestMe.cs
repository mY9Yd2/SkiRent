using System.Net;

using AutoFixture;

using Refit;

using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Systems.Controllers.Auth
{
    public class TestMe
    {
        private SkiRentWebApplicationFactory<Program> _factory;
        private ISkiRentApi _client;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _factory = new SkiRentWebApplicationFactory<Program>();
            _client = RestService.For<ISkiRentApi>(_factory.CreateClient());

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
            var user = TestDataHelper.CreateUser(_fixture);
            await _client.Users.CreateAsync(user.CreateUserRequest);
            await _client.Auth.SignInAsync(user.SignInRequest);

            // Act
            var response = await _client.Auth.MeAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUser()
        {
            // Arrange
            var user = TestDataHelper.CreateUser(_fixture);
            var createResponse = await _client.Users.CreateAsync(user.CreateUserRequest);

            var createdUser = createResponse.Content;
            Assert.That(createdUser, Is.Not.Null);

            await _client.Auth.SignInAsync(user.SignInRequest);

            var expectedResponse = new GetUserResponse
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                Role = createdUser.Role
            };

            // Act
            var response = await _client.Auth.MeAsync();
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
