using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Users
{
    public record UpdateUserRequest
    {
        public string? Email { get; init; }

        public string? Password { get; init; }

        public Roles? Role { get; init; }
    }
}
