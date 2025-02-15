using System.Security.Claims;

using FluentValidation;

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
    public async Task<IActionResult> AuthSignIn(
        [FromServices] IValidator<SignInRequest> validator, [FromBody] SignInRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _authService.SignInAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return SignIn(result.Value);
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
