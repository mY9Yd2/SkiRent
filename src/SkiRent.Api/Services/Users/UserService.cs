﻿using FluentResults;

using Microsoft.AspNetCore.Identity;

using SkiRent.Api.Data.Auth;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Extensions;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Api.Services.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<CreatedUserResponse>> CreateAsync(CreateUserRequest request, RoleTypes role = RoleTypes.Customer)
    {
        if (await _unitOfWork.Users.ExistsAsync(user => user.Email == request.Email))
        {
            return Result.Fail(new UserAlreadyExistsError(request.Email));
        }

        if (role == RoleTypes.Invalid)
        {
            return Result.Fail(new InvalidUserRoleError(role.ToString()));
        }

        var user = new User();

        user.Email = request.Email;
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        user.UserRole = role.ToRoleString();

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreatedUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = Enum.Parse<RoleTypes>(user.UserRole)
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
            Role = Enum.Parse<RoleTypes>(user.UserRole)
        };

        return Result.Ok(result);
    }

    public async Task<Result<IEnumerable<GetAllUserResponse>>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();

        var result = users.Select(user =>
            new GetAllUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = Enum.Parse<RoleTypes>(user.UserRole)
            });

        return Result.Ok(result);
    }

    public async Task<Result<GetUserResponse>> UpdateAsync(int userId, UpdateUserRequest request, Func<string, bool> isInRole)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
        {
            return Result.Fail(new UserNotFoundError(userId));
        }

        if (!isInRole(Roles.Admin))
        {
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPasswordAsNonNull);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Result.Fail(new PasswordVerificationFailedError());
            }
        }

        if (request.Email is not null)
        {
            if (await _unitOfWork.Users.ExistsAsync(user => user.Email == request.Email))
            {
                return Result.Fail(new UserAlreadyExistsError(request.Email));
            }
            user.Email = request.Email;
        }


        if (request.Password is not null)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        }

        if (request.Role.HasValue)
        {
            if (!isInRole(Roles.Admin))
            {
                return Result.Fail(new UnauthorizedModificationError(nameof(request.Role)));
            }
            user.UserRole = request.Role.Value.ToRoleString();
        }

        await _unitOfWork.SaveChangesAsync();

        var result = new GetUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = Enum.Parse<RoleTypes>(user.UserRole)
        };

        return Result.Ok(result);
    }

    public async Task<Result> DeleteAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
        {
            return Result.Fail(new UserNotFoundError(userId));
        }

        var hasActiveBookings = await _unitOfWork.Bookings.ExistsAsync(booking => booking.UserId == userId
            && booking.Status != BookingStatus.Cancelled
            && booking.Status != BookingStatus.Returned);

        if (hasActiveBookings)
        {
            return Result.Fail(new UserHasActiveBookingsError(userId));
        }

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
