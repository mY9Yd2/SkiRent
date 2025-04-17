using System.Security.Claims;

using AutoFixture;

using Microsoft.AspNetCore.Identity;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Auth;
using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.UnitTests.Systems.Api.Services.Auth
{
    public class TestSignInAsync
    {
        private IUnitOfWork _unitOfWork;
        private IPasswordHasher<User> _passwordHasher;
        private AuthService _authService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _authService = new AuthService(_unitOfWork, _passwordHasher);
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
            var user = _fixture.Create<User>();
            var request = _fixture.Build<SignInRequest>()
                .With(request => request.Email, user.Email)
                .Create();

            _unitOfWork.Users.GetByEmailAsync(request.Email)
                .Returns(user);
            _passwordHasher
                .VerifyHashedPassword(user, user.PasswordHash, request.Password)
                .Returns(PasswordVerificationResult.Success);

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
                Assert.That(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), Is.EqualTo(user.Id.ToString()));
                Assert.That(claimsPrincipal.FindFirstValue(ClaimTypes.Email), Is.EqualTo(user.Email));
                Assert.That(claimsPrincipal.FindFirstValue(ClaimTypes.Role), Is.EqualTo(user.UserRole));
            });
        }

        [Test]
        public async Task WhenUserDoesNotExist_ReturnsUserNotFoundError()
        {
            // Arrange
            var request = _fixture.Create<SignInRequest>();

            _unitOfWork.Users.GetByEmailAsync(request.Email)
                .Returns((User?)null);

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
            var user = _fixture.Create<User>();
            var request = _fixture.Build<SignInRequest>()
                .With(request => request.Email, user.Email)
                .Create();

            _unitOfWork.Users.GetByEmailAsync(request.Email)
                .Returns(user);
            _passwordHasher
                .VerifyHashedPassword(user, user.PasswordHash, request.Password)
                .Returns(PasswordVerificationResult.Failed);

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
