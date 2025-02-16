using System.Net;
using System.Net.Http.Json;

using AutoFixture;

using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Systems.Controllers.Users
{
    public class TestCreate
    {
        private SkiRentWebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _factory = new SkiRentWebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task WhenSuccessful_ReturnsCreatedStatus()
        {
            // Arrange
            var request = _fixture.Build<CreateUserRequest>()
                .With(request => request.Email, "teszt@example.com")
                .With(request => request.Password, "Teszt1234")
                .Create();

            // Act
            var response = await _client.PostAsJsonAsync("api/users", request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUser()
        {
            // Arrange
            var request = _fixture.Build<CreateUserRequest>()
                .With(request => request.Email, "teszt@example.com")
                .With(request => request.Password, "Teszt1234")
                .Create();
            var expectedResponse = new CreateUserResponse
            {
                Id = 1,
                Email = request.Email
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/users", request);
            var content = await response.Content.ReadFromJsonAsync<CreateUserResponse>();

            // Assert
            Assert.That(content, Is.EqualTo(expectedResponse));
        }
    }
}
