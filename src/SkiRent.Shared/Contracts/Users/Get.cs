namespace SkiRent.Shared.Contracts.Users
{
    public record GetUserResponse
    {
        public required int Id { get; init; }

        public required string Email { get; init; }

        public required string Role { get; init; }
    }
}
