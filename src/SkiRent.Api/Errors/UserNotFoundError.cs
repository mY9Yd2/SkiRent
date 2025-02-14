using FluentResults;

namespace SkiRent.Api.Errors;

public class UserNotFoundError : Error
{
    public UserNotFoundError(string email)
        : base($"User with email '{email}' not found.")
    {
        Metadata.Add(nameof(email), email);
    }

    public UserNotFoundError(int userId)
        : base($"User with id '{userId}' not found.")
    {
        Metadata.Add(nameof(userId), userId);
    }
}
