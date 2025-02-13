using System.Security.Claims;

using FluentResults;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<Result<ClaimsPrincipal>> SignInAsync(SignInRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Fail(new UserNotFoundError(request.Email));
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Result.Fail(new PasswordVerificationFailedError());
        }

        var result = CreatePrincipal(user.Email, user.UserRole);

        return Result.Ok(result);
    }

    private static ClaimsPrincipal CreatePrincipal(string email, string role)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role),
            };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identity);
    }
}
