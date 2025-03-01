using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Authorization.Requirements;
using SkiRent.Api.Data.Auth;

namespace SkiRent.Api.Configurations;

public static class AuthorizationPoliciesConfiguration
{
    public static void Configure(AuthorizationOptions options)
    {
        options.DefaultPolicy = BuildDefaultPolicy();
        options.AddPolicy(Policies.SelfOrAdminAccess, BuildSelfOrAdminAccessPolicy);
        options.AddPolicy(Policies.CustomerOrAdminAccess, BuildCustomerOrAdminAccessPolicy);
        options.AddPolicy(Policies.PaymentGatewayOnly, BuildPaymentGatewayOnlyPolicy);
    }

    private static AuthorizationPolicy BuildDefaultPolicy()
    {
        string[] authenticationSchemes = [
            CookieAuthenticationDefaults.AuthenticationScheme,
            BearerTokenDefaults.AuthenticationScheme
        ];

        var defaultPolicy = new AuthorizationPolicyBuilder(authenticationSchemes)
            .RequireAuthenticatedUser()
            .Build();

        return defaultPolicy;
    }

    private static void BuildPaymentGatewayOnlyPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddRequirements(new PaymentGatewayOnlyRequirement());
    }

    private static void BuildSelfOrAdminAccessPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddRequirements(new SelfOrAdminAccessRequirement());
    }

    private static void BuildCustomerOrAdminAccessPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddRequirements(new CustomerOrAdminAccessRequirement());
    }
}
