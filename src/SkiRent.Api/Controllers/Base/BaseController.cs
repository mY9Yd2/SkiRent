using FluentResults;

using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Errors;
using SkiRent.Api.Exceptions;

namespace SkiRent.Api.Controllers.Base;

public abstract class BaseController : ControllerBase
{
    [NonAction]
    public ObjectResult Problem(IError error)
    {
        switch (error)
        {
            case UserAlreadyExistsError:
                return Problem(
                        title: "Conflict",
                        detail: error.Message,
                        statusCode: StatusCodes.Status409Conflict);
            default:
                throw new UnhandledErrorException($"Unhandled error type: {error.GetType().Name}.");
        }
    }
}
