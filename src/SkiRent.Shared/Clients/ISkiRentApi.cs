using Refit;

using SkiRent.Shared.Contracts.Auth;
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

        public IAuthApi Auth => RestService.For<IAuthApi>(Client);
        public IUsersApi Users => RestService.For<IUsersApi>(Client);
    }

    public interface IAuthApi
    {
        [Post("/api/auth/sign-in")]
        public Task<IApiResponse> SignInAsync(SignInRequest request);

        [Post("/api/auth/sign-out")]
        public Task<IApiResponse> SignOutAsync(object empty);

        [Get("/api/auth/me")]
        public Task<IApiResponse<GetUserResponse>> Me();
    }

    public interface IUsersApi
    {
        [Post("/api/users")]
        public Task<IApiResponse<CreateUserResponse>> CreateAsync(CreateUserRequest request);

        [Get("/api/users/{userId}")]
        public Task<IApiResponse<GetUserResponse>> GetAsync(int userId);

        [Get("/api/users")]
        public Task<IApiResponse<IEnumerable<GetAllUserResponse>>> GetAllAsync();

        [Put("/api/users/{userId}")]
        public Task<IApiResponse<GetUserResponse>> UpdateAsync(int userId, UpdateUserRequest request);
    }
}
