using SkiRent.Api.Exceptions;
using SkiRent.Shared.Contracts.Common;

using AuthRoles = SkiRent.Api.Data.Auth.Roles;

namespace SkiRent.Api.Extensions;

public static class RolesExtensions
{
    public static string ToRoleString(this Roles role)
    {
        return role switch
        {
            Roles.Customer => AuthRoles.Customer,
            Roles.Admin => AuthRoles.Admin,
            _ => throw new UnhandledRoleException($"Unhandled role type: {role}.")
        };
    }
}
