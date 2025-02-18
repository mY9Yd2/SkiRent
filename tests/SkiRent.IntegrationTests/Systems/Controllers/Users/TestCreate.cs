using System.Net;

using AutoFixture;

using Refit;

using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Systems.Controllers.Users
{
    public class TestCreate
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
        public async Task WhenSuccessful_ReturnsCreatedStatus()
        {
            // Arrange
            var user = TestDataHelper.CreateUser(_fixture);

            // Act
            var response = await _client.Users.CreateAsync(user.CreateUserRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUser()
        {
            // Arrange
            var user = TestDataHelper.CreateUser(_fixture);
            var expectedResponse = new CreateUserResponse
            {
                Id = 1,
                Email = user.CreateUserRequest.Email,
                Role = Roles.Customer
            };

            // Act
            var response = await _client.Users.CreateAsync(user.CreateUserRequest);

            // Assert
            Assert.That(response.Content, Is.EqualTo(expectedResponse));
        }
    }
}
