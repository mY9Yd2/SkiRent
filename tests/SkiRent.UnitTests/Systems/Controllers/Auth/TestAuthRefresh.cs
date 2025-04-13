using System.Security.Claims;

using AutoFixture;

using FluentResults;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Controllers;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Auth;

namespace SkiRent.UnitTests.Systems.Controllers.Auth;

public class TestAuthRefresh
{
    private Fixture _fixture;
    private AuthController _controller;
    private IAuthService _authService;
    private IOptionsMonitor<BearerTokenOptions> _optionsMonitor;
    private TimeProvider _timeProvider;
    private BearerTokenOptions _bearerTokenOptions;
    private ISecureDataFormat<AuthenticationTicket> _protector;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();

        _authService = Substitute.For<IAuthService>();
        _optionsMonitor = Substitute.For<IOptionsMonitor<BearerTokenOptions>>();
        _protector = Substitute.For<ISecureDataFormat<AuthenticationTicket>>();
        _timeProvider = Substitute.For<TimeProvider>();

        _bearerTokenOptions = new BearerTokenOptions
        {
            RefreshTokenProtector = _protector
        };

        _optionsMonitor.Get(BearerTokenDefaults.AuthenticationScheme)
            .Returns(_bearerTokenOptions);

        _controller = new AuthController(_authService);
    }

    [Test]
    public void ExpiredRefreshToken_ReturnsChallenge()
    {
        // Arrange
        var request = _fixture.Create<RefreshRequest>();
        var pastTime = DateTimeOffset.UtcNow.AddMinutes(-5);

        var authTicket = new AuthenticationTicket(
            new ClaimsPrincipal(),
            new AuthenticationProperties { ExpiresUtc = pastTime },
            CookieAuthenticationDefaults.AuthenticationScheme);

        _protector.Unprotect(request.RefreshToken)
            .Returns(authTicket);
        _timeProvider.GetUtcNow()
            .Returns(DateTimeOffset.UtcNow);

        // Act
        var result = _controller.AuthRefresh(_optionsMonitor, _timeProvider, request);

        // Assert
        Assert.That(result, Is.TypeOf<ChallengeResult>());
    }

    [Test]
    public void ValidRefreshToken_ReturnsSignIn()
    {
        // Arrange
        var request = _fixture.Create<RefreshRequest>();
        var futureTime = DateTimeOffset.UtcNow.AddMinutes(10);

        var claim = new Claim(ClaimTypes.NameIdentifier, "123");
        var identity = new ClaimsIdentity([claim], "mock");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var authTicket = new AuthenticationTicket(
            claimsPrincipal,
            new AuthenticationProperties { ExpiresUtc = futureTime },
            CookieAuthenticationDefaults.AuthenticationScheme);

        _protector.Unprotect(request.RefreshToken)
            .Returns(authTicket);
        _timeProvider.GetUtcNow()
            .Returns(DateTimeOffset.UtcNow);

        var signInPrincipal = new ClaimsPrincipal();

        _authService.CreatePrincipal(claimsPrincipal)
            .Returns(Result.Ok(signInPrincipal));

        // Act
        var result = _controller.AuthRefresh(_optionsMonitor, _timeProvider, request);

        // Assert
        Assert.That(result, Is.TypeOf<SignInResult>());
        var signInResult = result as SignInResult;
        Assert.That(signInResult!.Principal, Is.EqualTo(signInPrincipal));
        Assert.That(signInResult.AuthenticationScheme, Is.EqualTo(BearerTokenDefaults.AuthenticationScheme));
    }

    [Test]
    public void InvalidPrincipal_ReturnsProblem()
    {
        // Arrange
        var request = _fixture.Create<RefreshRequest>();
        var futureTime = DateTimeOffset.UtcNow.AddMinutes(10);

        var claimsPrincipal = new ClaimsPrincipal();
        var authTicket = new AuthenticationTicket(
            claimsPrincipal,
            new AuthenticationProperties { ExpiresUtc = futureTime },
            CookieAuthenticationDefaults.AuthenticationScheme);

        _protector.Unprotect(request.RefreshToken)
            .Returns(authTicket);
        _timeProvider.GetUtcNow()
            .Returns(DateTimeOffset.UtcNow);

        _authService.CreatePrincipal(claimsPrincipal)
            .Returns(Result.Fail(new PasswordVerificationFailedError()));

        // Act
        var result = _controller.AuthRefresh(_optionsMonitor, _timeProvider, request);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }
}
