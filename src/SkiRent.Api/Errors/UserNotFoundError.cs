using FluentResults;

namespace SkiRent.Api.Errors;

public class UserNotFoundError : Error
{
    public UserNotFoundError(string email)
        : base($"User with email '{email}' not found.")
    {
        Metadata.Add(nameof(email), email);
    }
}
