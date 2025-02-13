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
            case PasswordVerificationFailedError:
                return Problem(
                        title: "Unauthorized",
                        detail: error.Message,
                        statusCode: StatusCodes.Status401Unauthorized);
            case UserNotFoundError:
                return Problem(
                        title: "Not Found",
                        detail: error.Message,
                        statusCode: StatusCodes.Status404NotFound);
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
