using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.Users;
using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserResponse>> SignIn(
        [FromServices] IValidator<SignInRequest> validator, [FromBody] SignInRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _authService.SignInAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Value);

        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> Me([FromServices] IUserService userService)
    {
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
