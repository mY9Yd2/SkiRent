using System.Text.Json.Serialization;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Users
{
    public record UpdateUserRequest
    {
        public string? Email { get; init; }

        public string? Password { get; init; }

        public Roles? Role { get; init; }

        [JsonIgnore]
        public string EmailAsNonNull => Email ?? string.Empty;

        [JsonIgnore]
        public string PasswordAsNonNull => Password ?? string.Empty;

        [JsonIgnore]
        public Roles RoleAsNonNull => Role.GetValueOrDefault(Roles.Invalid);
    }
}
