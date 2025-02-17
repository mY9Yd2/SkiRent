using System.Net;

using AutoFixture;

using Refit;

using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.IntegrationTests.Systems.Controllers.Auth
{
    public class TestSignOut
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
            var createRequest = TestDataCustomizationHelper.CreateUser(_fixture);
            await _client.Users.CreateAsync(createRequest);

            var request = _fixture.Create<SignInRequest>();
            await _client.Auth.SignInAsync(request);

            // Act
            var response = await _client.Auth.SignOutAsync(string.Empty);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
