using FluentResults;

using Microsoft.AspNetCore.Identity;

using SkiRent.Api.Data.Auth;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Services.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<Result<CreateUserResponse>> CreateAsync(CreateUserRequest request)
    {
        if (await _unitOfWork.Users.ExistsAsync(user => user.Email == request.Email))
        {
            return Result.Fail(new UserAlreadyExistsError(request.Email));
        }

        var user = new User();

        user.Email = request.Email;
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        user.UserRole = Roles.Customer;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreateUserResponse
        {
            Id = user.Id,
            Email = user.Email
        };

        return Result.Ok(result);
    }

    public async Task<Result<GetUserResponse>> GetAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
        {
            return Result.Fail(new UserNotFoundError(userId));
        }

        var result = new GetUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.UserRole
        };

        return Result.Ok(result);
    }
}
