using Refit;

using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Shared.Clients
{
    public interface ISkiRentApi
    {
        public HttpClient Client { get; }

        // Workaround:
        // Refit currently does not support to compose multiple interfaces in one API interface
        [Get("/api")]
        public Task<IApiResponse> PingAsync();

        public IUsersApi Users => RestService.For<IUsersApi>(Client);
    }

    public interface IUsersApi
    {
        [Post("/api/users")]
        public Task<IApiResponse<CreateUserResponse>> CreateAsync(CreateUserRequest request);
    }
}
