using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Users
{
    public record GetAllUserResponse
    {
        public required int Id { get; init; }

        public required string Email { get; init; }

        public required RoleTypes Role { get; init; }
    }
}
