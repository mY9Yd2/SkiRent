using System.Linq.Expressions;

using AutoFixture;

using Microsoft.AspNetCore.Identity;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Users;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.UnitTests.Systems.Api.Services.Users;

public class TestUpdateAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IUserService _userService;
    private IPasswordHasher<User> _passwordHasher;

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
        _userService = new UserService(_unitOfWork, _passwordHasher);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
    }

    [Test]
    public async Task WhenUserNotFound_ReturnsFailedResult()
    {
        // Arrange
        var userId = 1;
        var request = _fixture.Create<UpdateUserRequest>();

        _unitOfWork.Users
            .GetByIdAsync(userId)
            .Returns((User?)null);

        // Act
        var result = await _userService.UpdateAsync(userId, request, null!);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("userId"), Is.EqualTo(userId));
    }

    [Test]
    public async Task WhenPasswordVerificationFails_ReturnsFailedResult()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(user => user.PasswordHash, string.Empty)
            .Create();
        var request = _fixture.Create<UpdateUserRequest>();
        Func<string, bool> isInRole = _ => false;

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);
        _passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, request.CurrentPasswordAsNonNull)
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _userService.UpdateAsync(user.Id, request, isInRole);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<PasswordVerificationFailedError>());
    }

    [Test]
    public async Task WhenEmailExists_ReturnsFailedResult()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(user => user.PasswordHash, string.Empty)
            .Create();
        var request = _fixture.Create<UpdateUserRequest>();
        Func<string, bool> isInRole = _ => false;

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);
        _passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, request.CurrentPasswordAsNonNull)
            .Returns(PasswordVerificationResult.Success);
        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(true);

        // Act
        var result = await _userService.UpdateAsync(user.Id, request, isInRole);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserAlreadyExistsError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("email"), Is.EqualTo(request.Email));
    }

    [Test]
    public async Task WhenNonAdminTriesToChangeRole_ReturnsUnauthorizedError()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(user => user.PasswordHash, string.Empty)
            .Create();
        var request = _fixture.Create<UpdateUserRequest>();
        Func<string, bool> isInRole = _ => false;

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);
        _passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, request.CurrentPasswordAsNonNull)
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await _userService.UpdateAsync(user.Id, request, isInRole);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UnauthorizedModificationError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("resource"), Is.EqualTo(nameof(request.Role)));
    }

    [Test]
    public async Task WhenUserExists_DeletesUserAndSavesChanges()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);
        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(false);

        _unitOfWork.Bookings.ExistsAsync(
            Arg.Any<Expression<Func<Booking, bool>>>())
            .Returns(call =>
            {
                var expression = call.Arg<Expression<Func<Booking, bool>>>();
                var predicate = expression.Compile();

                // Simulate bookings with different combinations
                var bookings = new[]
                {
                    new Booking { UserId = user.Id, Status = BookingStatus.Cancelled },
                    new Booking { UserId = user.Id, Status = BookingStatus.Returned },
                    new Booking { UserId = _fixture.Create<int>(), Status = BookingStatus.Paid }
                };

                return bookings.Any(predicate);
            });

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _unitOfWork.Users.Received(1).Delete(user);
        await _unitOfWork.Received(1).SaveChangesAsync();
        await _unitOfWork.Bookings.Received(1).ExistsAsync(
            Arg.Is<Expression<Func<Booking, bool>>>(expression =>
                expression.Compile().Invoke(new Booking
                {
                    UserId = user.Id,
                    Status = BookingStatus.Paid
                }))
            );
    }
}
