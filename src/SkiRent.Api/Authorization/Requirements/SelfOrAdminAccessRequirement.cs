using Microsoft.AspNetCore.Authorization;

namespace SkiRent.Api.Authorization.Requirements;

public class SelfOrAdminAccessRequirement : IAuthorizationRequirement
{ }
