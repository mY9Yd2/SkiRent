using System.Net;

using AutoFixture;

using Refit;

using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Auth;
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
            var createRequest = _fixture.Build<CreateUserRequest>()
                .With(request => request.Email, "teszt@example.com")
                .With(request => request.Password, "Teszt1234")
                .Create();
            await _client.Users.CreateAsync(createRequest);

            var request = _fixture.Build<SignInRequest>()
                .With(request => request.Email, createRequest.Email)
                .With(request => request.Password, createRequest.Password)
                .Create();
            await _client.Auth.SignInAsync(request);

            // Act
            var response = await _client.Auth.Me();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenSuccessful_ReturnsUser()
        {
            // Arrange
            var createRequest = _fixture.Build<CreateUserRequest>()
                .With(request => request.Email, "teszt@example.com")
                .With(request => request.Password, "Teszt1234")
                .Create();
            var createResponse = await _client.Users.CreateAsync(createRequest);
            var createResponseContent = createResponse.Content;

            var request = _fixture.Build<SignInRequest>()
                .With(request => request.Email, createRequest.Email)
                .With(request => request.Password, createRequest.Password)
                .Create();
            await _client.Auth.SignInAsync(request);

            // Act
            var response = await _client.Auth.Me();
            var content = response.Content;

            // Assert
            Assert.That(content, Is.Not.Null);
            Assert.That(createResponseContent, Is.Not.Null);

            Assert.That(content.Id, Is.EqualTo(createResponseContent.Id));
            Assert.That(content.Email, Is.EqualTo(request.Email));
            Assert.That(content.Role, Is.EqualTo(createResponseContent.Role));
        }
    }
}
