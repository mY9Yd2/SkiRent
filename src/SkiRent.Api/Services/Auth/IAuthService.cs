using System.Security.Claims;

using FluentResults;

using SkiRent.Shared.Contracts.Auth;

namespace SkiRent.Api.Services.Auth;

public interface IAuthService
{
    public string AuthenticationScheme { get; set; }
    public Task<Result<ClaimsPrincipal>> SignInAsync(SignInRequest request);
    public Result<ClaimsPrincipal> CreatePrincipal(ClaimsPrincipal principal);
}
