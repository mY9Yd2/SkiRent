using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Users;

namespace SkiRent.UnitTests.Systems.Api.Services.Users;

public class TestDeleteAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IUserService _userService;

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
        _userService = new UserService(_unitOfWork, null!);
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
        var userId = _fixture.Create<int>();

        _unitOfWork.Users
            .GetByIdAsync(userId)
            .Returns((User?)null);

        // Act
        var result = await _userService.DeleteAsync(userId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("userId"), Is.EqualTo(userId));
    }

    [Test]
    public async Task WhenUserHasActiveBookings_ReturnsFailedResult()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);

        _unitOfWork.Bookings
            .ExistsAsync(Arg.Any<Expression<Func<Booking, bool>>>())
            .Returns(true);

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserHasActiveBookingsError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("userId"), Is.EqualTo(user.Id));
    }

    [Test]
    public async Task WhenUserExists_DeletesUserAndSavesChanges()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _unitOfWork.Users
            .GetByIdAsync(user.Id)
            .Returns(user);

        _unitOfWork.Bookings
            .ExistsAsync(Arg.Any<Expression<Func<Booking, bool>>>())
            .Returns(false);

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        Assert.That(result.IsFailed, Is.False);
        _unitOfWork.Users.Received(1).Delete(user);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
