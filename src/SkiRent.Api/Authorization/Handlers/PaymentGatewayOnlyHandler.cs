using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using SkiRent.Api.Authorization.Requirements;
using SkiRent.Api.Configurations;

namespace SkiRent.Api.Authorization.Handlers;

public class PaymentGatewayOnlyHandler : AuthorizationHandler<PaymentGatewayOnlyRequirement>
{
    private readonly PaymentGatewayOptions _paymentGatewayOptions;

    public PaymentGatewayOnlyHandler(IOptions<PaymentGatewayOptions> paymentGatewayOptions)
    {
        _paymentGatewayOptions = paymentGatewayOptions.Value;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PaymentGatewayOnlyRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        if (!httpContext.Request.Headers.TryGetValue("X-Signature", out var signature) || StringValues.IsNullOrEmpty(signature))
        {
            context.Fail();
            return;
        }

        httpContext.Request.EnableBuffering();

        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
        var requestBody = await reader.ReadToEndAsync();

        httpContext.Request.Body.Position = 0;

        if (string.IsNullOrEmpty(requestBody))
        {
            context.Fail();
            return;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_paymentGatewayOptions.SharedSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
        var signatureBytes = Convert.FromBase64String(signature.ToString());

        if (CryptographicOperations.FixedTimeEquals(hashBytes, signatureBytes))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
