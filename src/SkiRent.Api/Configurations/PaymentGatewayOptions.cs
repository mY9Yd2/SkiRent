namespace SkiRent.Api.Configurations;

public class PaymentGatewayOptions
{
    public required Uri BaseUrl { get; set; }

    public required string SharedSecret { get; set; }
}
