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
    public void Get([FromRoute] int bookingId)
    {
        throw new NotImplementedException();
    }
}
