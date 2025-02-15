using FluentResults;

using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Services.Users;

public interface IUserService
{
    public Task<Result<CreateUserResponse>> CreateAsync(CreateUserRequest request);
    public Task<Result<GetUserResponse>> GetAsync(int userId);
    public Task<Result<IEnumerable<GetAllUserResponse>>> GetAllAsync();
    public Task<Result<GetUserResponse>> UpdateAsync(int userId, UpdateUserRequest request, Func<string, bool> isInRole);
}
