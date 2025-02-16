using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SkiRent.Api.Data;

public partial class SkiRentContext : DbContext
{
    private readonly ILogger<SkiRentContext> _logger = null!;
    private readonly SkiRentContextSettings _settings = null!;

    public SkiRentContext(
        DbContextOptions<SkiRentContext> options,
        ILogger<SkiRentContext> logger,
        IOptions<SkiRentContextSettings> settings)
        : base(options)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_settings.ConnectionString, ServerVersion.AutoDetect(_settings.ConnectionString));
        }

        optionsBuilder.EnableDetailedErrors();

        if (_settings.IsDevelopment)
        {
            optionsBuilder.LogTo(logMessage => _logger.LogInformation("{LogMessage}", logMessage), LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
