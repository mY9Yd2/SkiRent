namespace SkiRent.Api.Data;

public class SkiRentContextSettings
{
    public required bool IsDevelopment { get; set; }

    public required string ConnectionString { get; set; }
}
