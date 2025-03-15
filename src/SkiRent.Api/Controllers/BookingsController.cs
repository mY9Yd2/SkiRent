using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.Bookings;
using SkiRent.Shared.Contracts.Bookings;

namespace SkiRent.Api.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingsController : BaseController
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Customer)]
    public async Task<ActionResult<CreatedBookingResponse>> Create(
        [FromServices] IValidator<CreateBookingRequest> validator, [FromBody] CreateBookingRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(nameIdentifier, out int userId))
        {
            return Unauthorized();
        }

        var result = await _bookingService.CreateAsync(userId, request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return CreatedAtAction(nameof(Get), new { bookingId = result.Value.Id }, result.Value);
    }

    [HttpGet("{bookingId:int}")]
    [Authorize(Policy = Policies.CustomerOrAdminAccess)]
    public async Task<ActionResult<GetBookingResponse>> Get([FromRoute] int bookingId)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(nameIdentifier, out int userId))
        {
            return Unauthorized();
        }

        var result = await _bookingService.GetAsync(bookingId, userId, User.IsInRole);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = Policies.CustomerOrAdminAccess)]
    public async Task<ActionResult<IEnumerable<GetAllBookingResponse>>> GetAll()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(nameIdentifier, out int userId))
        {
            return Unauthorized();
        }

        var result = await _bookingService.GetAllAsync(userId, User.IsInRole);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpPut("{bookingId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<GetBookingResponse>> Update(
        [FromServices] IValidator<UpdateBookingRequest> validator, [FromRoute] int bookingId, [FromBody] UpdateBookingRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _bookingService.UpdateAsync(bookingId, request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{bookingId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete([FromRoute] int bookingId)
    {
        var result = await _bookingService.DeleteAsync(bookingId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok();
    }
}
