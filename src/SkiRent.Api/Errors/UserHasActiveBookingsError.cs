using FluentResults;

namespace SkiRent.Api.Errors;

public class UserHasActiveBookingsError : Error
{
    public UserHasActiveBookingsError(int userId)
        : base($"User with id '{userId}' cannot be deleted due to active bookings.")
    {
        Metadata.Add(nameof(userId), userId);
    }
}
