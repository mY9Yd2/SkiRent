using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Users
{
    public record CreateUserRequest
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
    }

    public record CreateUserResponse
    {
        public required int Id { get; init; }

        public required string Email { get; init; }

        public required Roles Role { get; init; }
    }
}
