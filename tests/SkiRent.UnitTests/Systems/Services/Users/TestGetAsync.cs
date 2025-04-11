using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Users;

namespace SkiRent.UnitTests.Systems.Services.Users;

public class TestGetAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IUserService _userService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
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

        _unitOfWork.Users.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _userService.GetAsync(userId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("userId"), Is.EqualTo(userId));
    }
}
