using FluentResults;

namespace SkiRent.Api.Errors;

public class UserAlreadyExistsError : Error
{
    public UserAlreadyExistsError(string email)
        : base($"A user with email '{email}' already exists.")
    {
        Metadata.Add(nameof(email), email);
    }
}
