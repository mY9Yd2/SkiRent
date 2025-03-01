namespace SkiRent.Api.Data.Auth;

public static class Policies
{
    public const string SelfOrAdminAccess = "SelfOrAdminAccess";
    public const string CustomerOrAdminAccess = "CustomerOrAdminAccess";
    public const string PaymentGatewayOnly = "PaymentGatewayOnly";
}
