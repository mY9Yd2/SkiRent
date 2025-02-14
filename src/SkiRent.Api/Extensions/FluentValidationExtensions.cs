using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SkiRent.Api.Extensions;

public static class FluentValidationExtensions
{
    public static ModelStateDictionary ToModelStateDictionary(this ValidationResult result)
    {
        var modelState = new ModelStateDictionary();

        result.Errors
            .GroupBy(error => error.PropertyName)
            .SelectMany(group => group.Select(error =>
            {
                if (error.FormattedMessagePlaceholderValues is not null)
                {
                    // Workaround
                    var property = error.FormattedMessagePlaceholderValues["PropertyName"] ?? error.PropertyName;
                    error.ErrorMessage = error.ErrorMessage.Replace("''", $"'{property}'");
                }

                return new { error.PropertyName, error.ErrorMessage };
            }))
            .ToList()
            .ForEach(error => modelState.AddModelError(error.PropertyName, error.ErrorMessage));

        return modelState;
    }
}
