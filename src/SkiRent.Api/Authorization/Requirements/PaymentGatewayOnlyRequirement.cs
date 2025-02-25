using Microsoft.AspNetCore.Authorization;

namespace SkiRent.Api.Authorization.Requirements;

public class PaymentGatewayOnlyRequirement : IAuthorizationRequirement
{ }
