using FluentResults;

namespace SkiRent.Api.Errors;

public class UnauthorizedModificationError : Error
{
    public UnauthorizedModificationError(string resource)
        : base($"Unauthorized modification attempt on '{resource}'.")
    {
        Metadata.Add(nameof(resource), resource);
    }
}
