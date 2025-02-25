using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Authorization.Requirements;
using SkiRent.Api.Data.Auth;

namespace SkiRent.Api.Authorization.Handlers;

public class SelfOrAdminAccessHandler : AuthorizationHandler<SelfOrAdminAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfOrAdminAccessRequirement requirement)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);

        if (roleClaim == Roles.Admin)
        {
            // Admins can view any user
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (roleClaim == Roles.Customer && userIdClaim is not null)
        {
            if (context.Resource is not HttpContext httpContext)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!httpContext.Request.RouteValues.TryGetValue("userId", out var routeUserIdObj) || routeUserIdObj is null)
            {
                // No userId in route
                context.Fail();
                return Task.CompletedTask;
            }

            if (!int.TryParse(routeUserIdObj.ToString(), out int requestedUserId))
            {
                // Invalid userId format
                context.Fail();
                return Task.CompletedTask;
            }

            if (userIdClaim == requestedUserId.ToString())
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
