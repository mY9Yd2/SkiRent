using FluentResults;

namespace SkiRent.Api.Errors;

public class InvalidUserRoleError : Error
{
    public InvalidUserRoleError(string role)
        : base($"Invalid user role: '{role}'.")
    {
        Metadata.Add(nameof(role), role);
    }
}
