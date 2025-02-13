using FluentResults;

using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Services.Users;

public interface IUserService
{
    public Task<Result<CreateUserResponse>> CreateAsync(CreateUserRequest request);
    public Task<Result<GetUserResponse>> GetAsync(int userId);
}
