using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.Models
{
    public record UserList
    {
        public required int Id { get; init; }

        public required string Email { get; init; }

        public required RoleTypes Role { get; init; }
    }
}
