using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.Users;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserResponse>> Create(
        [FromServices] IValidator<CreateUserRequest> validator, [FromBody] CreateUserRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _userService.CreateAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return CreatedAtAction(nameof(Get), new { userId = result.Value.Id }, result.Value);
    }

    [HttpGet("{userId:int}")]
    [Authorize(Policy = Policies.SelfOrAdminAccess)]
    public async Task<ActionResult<GetUserResponse>> Get([FromRoute] int userId)
    {
        var result = await _userService.GetAsync(userId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IEnumerable<GetAllUserResponse>>> GetAll()
    {
        var result = await _userService.GetAllAsync();

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpPut("{userId:int}")]
    [Authorize(Policy = Policies.SelfOrAdminAccess)]
    public async Task<ActionResult<GetUserResponse>> Update(
        [FromServices] IValidator<UpdateUserRequest> validator, [FromRoute] int userId, UpdateUserRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _userService.UpdateAsync(userId, request, User.IsInRole);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }
}
