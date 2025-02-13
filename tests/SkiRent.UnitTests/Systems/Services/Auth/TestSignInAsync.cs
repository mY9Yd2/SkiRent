using System.Security.Claims;

using AutoFixture;

using Microsoft.AspNetCore.Identity;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Auth;
using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.UnitTests.Systems.Services.Auth
{
    class TestSignInAsync
    {
        private IUnitOfWork _unitOfWork;
        private PasswordHasher<User> _passwordHasher;
        private AuthService _authService;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = new PasswordHasher<User>();
            _authService = new AuthService(_unitOfWork);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }

        [Test]
        public async Task WhenUserExistsAndPasswordIsValid_ReturnsClaimsPrincipal()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var user = _fixture.Build<User>()
                               .With(user => user.Email, email)
                               .With(user => user.PasswordHash, _passwordHasher.HashPassword(null!, password))
                               .Create();
            var request = new SignInRequest { Email = email, Password = password };

            _unitOfWork.Users.GetByEmailAsync(Arg.Any<string>()).Returns(user);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Is.InstanceOf<ClaimsPrincipal>());
            });

            var claimsPrincipal = result.Value;

            Assert.That(claimsPrincipal, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value, Is.EqualTo(email));
                Assert.That(claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value, Is.EqualTo(user.UserRole));
            });
        }

        [Test]
        public async Task WhenUserDoesNotExist_ReturnsUserNotFoundError()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var request = new SignInRequest { Email = email, Password = password };

            _unitOfWork.Users.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Errors[0], Is.InstanceOf<UserNotFoundError>());
            });
        }

        [Test]
        public async Task WhenPasswordIsIncorrect_ReturnsPasswordVerificationFailedError()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var incorrectPassword = _fixture.Create<string>();
            var user = _fixture.Build<User>()
                               .With(user => user.Email, email)
                               .With(user => user.PasswordHash, _passwordHasher.HashPassword(null!, password))
                               .Create();
            var request = new SignInRequest { Email = email, Password = incorrectPassword };

            _unitOfWork.Users.GetByEmailAsync(Arg.Any<string>()).Returns(user);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Errors[0], Is.InstanceOf<PasswordVerificationFailedError>());
            });
        }
    }
}
