using System.Security.Claims;

using FluentResults;

using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.Api.Services.Auth;

public interface IAuthService
{
    public Task<Result<ClaimsPrincipal>> SignInAsync(SignInRequest request);
}
