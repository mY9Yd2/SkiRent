using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SkiRent.Api.Errors;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Extensions;

namespace SkiRent.Api.Controllers.Base;

/// <summary>
/// Provides common behavior and utilities for API controllers, including error mapping and request validation.
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Converts a domain error into an appropriate <see cref="ObjectResult"/> based on its type.
    /// </summary>
    /// <param name="error">The domain error to convert.</param>
    /// <returns>An <see cref="ObjectResult"/> representing the corresponding HTTP response.</returns>
    /// <exception cref="UnhandledErrorException">Thrown when the error type is not explicitly handled.</exception>
    [NonAction]
    protected ObjectResult Problem(IError error)
    {
        switch (error)
        {
            case InvalidUserRoleError:
            case InvalidBookingStatusTransitionError:
                return Problem(
                        title: "Bad Request",
                        detail: error.Message,
                        statusCode: StatusCodes.Status400BadRequest);
            case MissingUserClaimError:
            case UnauthorizedModificationError:
            case PasswordVerificationFailedError:
                return Problem(
                        title: "Unauthorized",
                        detail: error.Message,
                        statusCode: StatusCodes.Status401Unauthorized);
            case BookingAccessDeniedError:
            case InvoiceAccessDeniedError:
                return Problem(
                        title: "Forbidden",
                        detail: error.Message,
                        statusCode: StatusCodes.Status403Forbidden);
            case BookingNotFoundError:
            case InvoiceNotFoundError:
            case EquipmentImageNotFoundError:
            case EquipmentCategoryNotFoundError:
            case EquipmentNotFoundError:
            case UserNotFoundError:
                return Problem(
                        title: "Not Found",
                        detail: error.Message,
                        statusCode: StatusCodes.Status404NotFound);
            case UserHasActiveBookingsError:
            case EquipmentImageInUseError:
            case BookingDeletionNotAllowedError:
            case EquipmentCategoryNotEmptyError:
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

    /// <summary>
    /// Validates the given request using the provided FluentValidation validator.
    /// </summary>
    /// <typeparam name="T">The type of the request to validate.</typeparam>
    /// <param name="validator">The validator used to validate the request.</param>
    /// <param name="request">The request object to validate.</param>
    /// <returns>
    /// A <see cref="ModelStateDictionary"/> containing validation errors if the request is invalid;
    /// otherwise, <c>null</c> if the validation passes.
    /// </returns>
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
