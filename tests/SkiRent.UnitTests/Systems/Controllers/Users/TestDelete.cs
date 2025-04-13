using System.Security.Claims;

using AutoFixture;

using FluentResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using SkiRent.Api.Controllers;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Services.Users;

namespace SkiRent.UnitTests.Systems.Controllers.Users;

public class TestDelete
{
    private Fixture _fixture;
    private IUserService _userService;
    private UsersController _controller;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();

        _userService = Substitute.For<IUserService>();

        _controller = new UsersController(_userService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, Roles.Admin)
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext.HttpContext.User = principal;
    }

    [Test]
    public async Task SelfDeleteAttempt_ReturnsForbid()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser(userId.ToString());

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.That(result, Is.TypeOf<ForbidResult>());
    }

    [Test]
    public void DeleteFails_ReturnsProblem()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser((userId + 1).ToString());

        var errorMessage = _fixture.Create<string>();

        _userService.DeleteAsync(userId)
            .Returns(Result.Fail(errorMessage));

        // Act & Assert
        Assert.ThrowsAsync<UnhandledErrorException>(async () => await _controller.Delete(userId));
    }

    [Test]
    public async Task Success_ReturnsOk()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser((userId + 1).ToString());

        _userService.DeleteAsync(userId)
            .Returns(Result.Ok());

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }
}
