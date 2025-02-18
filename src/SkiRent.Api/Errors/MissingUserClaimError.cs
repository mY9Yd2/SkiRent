using FluentResults;

namespace SkiRent.Api.Errors;

public class MissingUserClaimError : Error
{
    public MissingUserClaimError()
        : base("The required user claim is missing.")
    { }
}
