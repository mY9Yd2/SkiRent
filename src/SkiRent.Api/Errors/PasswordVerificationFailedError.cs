using FluentResults;

namespace SkiRent.Api.Errors;

public class PasswordVerificationFailedError : Error
{
    public PasswordVerificationFailedError()
        : base("Password verification failed.")
    {

    }
}
