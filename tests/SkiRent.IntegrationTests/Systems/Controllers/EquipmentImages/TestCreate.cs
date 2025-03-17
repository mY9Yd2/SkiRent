using System.Net;

using AutoFixture;

using Refit;

using SkiRent.Api.Services.Users;
using SkiRent.IntegrationTests.Utils;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.IntegrationTests.Systems.Controllers.EquipmentImages
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

            var formFile = TestDataHelper.CreateEquipmentImage();

            var streamContent = new StreamContent(formFile.OpenReadStream())
            {
                Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType) }
            };

            var formData = new MultipartFormDataContent
            {
                { streamContent, "formFile", formFile.FileName }
            };

            // Act
            var response = await _client.EquipmentImages.CreateAsync(formData);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }
    }
}
