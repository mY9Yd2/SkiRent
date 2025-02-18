using FluentResults;

using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Services.Users;

public interface IUserService
{
    public Task<Result<CreateUserResponse>> CreateAsync(CreateUserRequest request, Roles role = Roles.Customer);
    public Task<Result<GetUserResponse>> GetAsync(int userId);
    public Task<Result<IEnumerable<GetAllUserResponse>>> GetAllAsync();
    public Task<Result<GetUserResponse>> UpdateAsync(int userId, UpdateUserRequest request, Func<string, bool> isInRole);
}
