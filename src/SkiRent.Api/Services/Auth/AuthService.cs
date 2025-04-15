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
    private readonly IPasswordHasher<User> _passwordHasher;

    public string AuthenticationScheme { get; set; }

    public AuthService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;

        AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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

        var result = CreatePrincipal(user.Id, user.Email, user.UserRole);

        return Result.Ok(result);
    }

    public Result<ClaimsPrincipal> CreatePrincipal(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var role = principal.FindFirstValue(ClaimTypes.Role);

        if (userId is null || email is null || role is null)
        {
            return Result.Fail(new MissingUserClaimError());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var newPrincipal = new ClaimsPrincipal(identity);

        return Result.Ok(newPrincipal);
    }

    private ClaimsPrincipal CreatePrincipal(int userId, string email, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);

        return new ClaimsPrincipal(identity);
    }
}
