using System.Security.Claims;

using AutoFixture;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using SkiRent.Api.Controllers;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Services.Auth;
using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.UnitTests.Systems.Api.Controllers.Auth;

public class TestAuthSignIn
{
    private Fixture _fixture;
    private IValidator<SignInRequest> _validator;
    private IAuthService _authService;
    private AuthController _controller;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();

        _validator = Substitute.For<IValidator<SignInRequest>>();
        _authService = Substitute.For<IAuthService>();

        _controller = new AuthController(_authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Test]
    public async Task InvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var request = _fixture.Create<SignInRequest>();

        var validationFailure = new ValidationResult(
        [
            new ValidationFailure("Email", "Email is required")
        ]);

        _validator.ValidateAsync(request, default)
            .Returns(validationFailure);

        // Act
        var result = await _controller.AuthSignIn(_validator, request, null);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());

        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.Value, Is.TypeOf<ValidationProblemDetails>());

        var problemDetails = objectResult.Value as ValidationProblemDetails;
        Assert.That(problemDetails!.Errors.ContainsKey("Email"));
        Assert.That(problemDetails.Errors["Email"], Does.Contain("Email is required"));
    }

    [Test]
    public void ValidRequest_ButFails_ReturnsProblem()
    {
        // Arrange
        var request = _fixture.Create<SignInRequest>();

        _validator.ValidateAsync(request, default)
            .Returns(new ValidationResult());
        _authService.SignInAsync(request)
            .Returns(Result.Fail("Invalid credentials"));

        // Act & Assert
        Assert.ThrowsAsync<UnhandledErrorException>(async () => await _controller.AuthSignIn(_validator, request, useTokens: false));
    }

    [Test]
    public async Task WithCookies_Success_ReturnsSignIn()
    {
        // Arrange
        var request = _fixture.Create<SignInRequest>();

        var claim = new Claim(ClaimTypes.Name, "User");
        var identity = new ClaimsIdentity([claim]);
        var principal = new ClaimsPrincipal(identity);

        _validator.ValidateAsync(request, default)
            .Returns(new ValidationResult());
        _authService.SignInAsync(request)
            .Returns(Result.Ok(principal));

        // Act
        var result = await _controller.AuthSignIn(_validator, request, useTokens: null);

        // Assert
        var signInResult = result as SignInResult;
        Assert.That(signInResult, Is.Not.Null);
        Assert.That(signInResult!.AuthenticationScheme, Is.EqualTo(CookieAuthenticationDefaults.AuthenticationScheme));
        Assert.That(signInResult.Principal.Identity!.Name, Is.EqualTo("User"));
    }

    [Test]
    public async Task WithBearerToken_Success_ReturnsSignIn()
    {
        // Arrange
        var request = _fixture.Create<SignInRequest>();

        var claim = new Claim(ClaimTypes.Name, "BearerUser");
        var identity = new ClaimsIdentity([claim]);
        var principal = new ClaimsPrincipal(identity);

        _validator.ValidateAsync(request, default)
            .Returns(new ValidationResult());
        _authService.SignInAsync(request)
            .Returns(Result.Ok(principal));

        // Act
        var result = await _controller.AuthSignIn(_validator, request, useTokens: true);

        // Assert
        var signInResult = result as SignInResult;
        Assert.That(signInResult, Is.Not.Null);
        Assert.That(signInResult!.AuthenticationScheme, Is.EqualTo(BearerTokenDefaults.AuthenticationScheme));
        Assert.That(signInResult.Principal.Identity!.Name, Is.EqualTo("BearerUser"));
    }
}
