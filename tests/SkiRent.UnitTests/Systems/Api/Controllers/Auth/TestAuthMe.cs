using System.Security.Claims;

using AutoFixture;

using FluentResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using SkiRent.Api.Controllers;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.Users;

using SkiRent.Shared.Contracts.Users;

namespace SkiRent.UnitTests.Systems.Api.Controllers.Auth;

public class TestAuthMe
{
    private Fixture _fixture;
    private IAuthService _authService;
    private IUserService _userService;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _authService = Substitute.For<IAuthService>();
        _userService = Substitute.For<IUserService>();

        _controller = new AuthController(_authService)
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
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext.HttpContext.User = principal;
    }

    [Test]
    public async Task InvalidUserId_ReturnsUnauthorized()
    {
        // Arrange
        SetUser("not-an-int");

        // Act
        var result = await _controller.AuthMe(_userService);

        // Assert
        Assert.That(result.Result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public void UserServiceFails_ReturnsProblem()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser(userId.ToString());

        var errorMessage = _fixture.Create<string>();

        _userService.GetAsync(userId)
            .Returns(Result.Fail(errorMessage));

        // Act & Assert
        Assert.ThrowsAsync<UnhandledErrorException>(async () => await _controller.AuthMe(_userService));
    }

    [Test]
    public async Task Success_ReturnsOkWithUser()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser(userId.ToString());

        var userResponse = _fixture.Create<GetUserResponse>();

        _userService.GetAsync(userId)
            .Returns(Result.Ok(userResponse));

        // Act
        var result = await _controller.AuthMe(_userService);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(userResponse));
    }
}
