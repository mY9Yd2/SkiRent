using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SkiRent.Api.Errors;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Extensions;

namespace SkiRent.Api.Controllers.Base;

public abstract class BaseController : ControllerBase
{
    [NonAction]
    protected ObjectResult Problem(IError error)
    {
        switch (error)
        {
            case UnauthorizedModificationError:
            case PasswordVerificationFailedError:
                return Problem(
                        title: "Unauthorized",
                        detail: error.Message,
                        statusCode: StatusCodes.Status401Unauthorized);
            case PaymentNotFoundError:
            case EquipmentCategoryNotFound:
            case EquipmentNotFoundError:
            case UserNotFoundError:
                return Problem(
                        title: "Not Found",
                        detail: error.Message,
                        statusCode: StatusCodes.Status404NotFound);
            case EquipmentCategoryAlreadyExistsError:
            case UserAlreadyExistsError:
                return Problem(
                        title: "Conflict",
                        detail: error.Message,
                        statusCode: StatusCodes.Status409Conflict);
            case InsufficientQuantityError:
                return Problem(
                        title: "Unprocessable Entity",
                        detail: error.Message,
                        statusCode: StatusCodes.Status422UnprocessableEntity);
            default:
                throw new UnhandledErrorException($"Unhandled error type: {error.GetType().Name}.");
        }
    }

    [NonAction]
    protected async Task<ModelStateDictionary?> ValidateRequestAsync<T>(IValidator<T> validator, T request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            // Validation passed
            return null;
        }

        var modelState = validationResult.ToModelStateDictionary();
        return modelState;
    }
}
