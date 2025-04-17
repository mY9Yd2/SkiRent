using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Users;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.UnitTests.Systems.Api.Services.Users;

public class TestCreateAsync
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
    public async Task WhenUserAlreadyExists_ReturnsFailedResult()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();

        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(true);

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<UserAlreadyExistsError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("email"), Is.EqualTo(request.Email));
    }

    [Test]
    public async Task WhenInvalidUserRole_ReturnsFailedResult()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();
        var role = RoleTypes.Invalid;

        _unitOfWork.Users
            .ExistsAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(false);

        // Act
        var result = await _userService.CreateAsync(request, role);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<InvalidUserRoleError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("role"), Is.EqualTo(role.ToString()));
    }
}
