using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.Users;
using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> AuthSignIn(
        [FromServices] IValidator<SignInRequest> validator, [FromBody] SignInRequest request, [FromQuery] bool? useTokens)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        _authService.AuthenticationScheme = useTokens is not null
            ? BearerTokenDefaults.AuthenticationScheme
            : CookieAuthenticationDefaults.AuthenticationScheme;

        var result = await _authService.SignInAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return SignIn(result.Value, _authService.AuthenticationScheme);
    }

    [HttpPost("refresh")]
    [Authorize]
    public IActionResult AuthRefresh(
        [FromServices] IOptionsMonitor<BearerTokenOptions> bearerTokenOptions, [FromServices] TimeProvider timeProvider, [FromBody] RefreshRequest request)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(BearerTokenDefaults.AuthenticationScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        _authService.AuthenticationScheme = BearerTokenDefaults.AuthenticationScheme;

        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc || timeProvider.GetUtcNow() >= expiresUtc)
        {
            return Challenge(_authService.AuthenticationScheme);
        }

        var result = _authService.CreatePrincipal(refreshTicket.Principal);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return SignIn(result.Value, _authService.AuthenticationScheme);
    }

    [HttpPost("sign-out")]
    [Authorize]
    public IActionResult AuthSignOut([FromBody] object empty)
    {
        if (empty is not null)
        {
            return SignOut();
        }

        return Unauthorized();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> AuthMe([FromServices] IUserService userService)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(nameIdentifier, out int userId))
        {
            return Unauthorized();
        }

        var result = await userService.GetAsync(userId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }
}
