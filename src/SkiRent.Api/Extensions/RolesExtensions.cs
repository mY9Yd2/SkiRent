using SkiRent.Api.Data.Auth;
using SkiRent.Api.Exceptions;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Api.Extensions;

public static class RolesExtensions
{
    public static string ToRoleString(this RoleTypes role)
    {
        return role switch
        {
            RoleTypes.Customer => Roles.Customer,
            RoleTypes.Admin => Roles.Admin,
            _ => throw new UnhandledRoleException($"Unhandled role type: {role}.")
        };
    }
}
