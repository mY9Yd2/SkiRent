using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Authorization.Requirements;
using SkiRent.Api.Data.Auth;

namespace SkiRent.Api.Authorization.Handlers;

public class CustomerOrAdminAccessHandler : AuthorizationHandler<CustomerOrAdminAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerOrAdminAccessRequirement requirement)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);

        if (roleClaim == Roles.Admin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (roleClaim == Roles.Customer && userIdClaim is not null)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
