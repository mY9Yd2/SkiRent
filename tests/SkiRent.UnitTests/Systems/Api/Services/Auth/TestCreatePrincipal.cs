using System.Security.Claims;

using NSubstitute;

using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Auth;

namespace SkiRent.UnitTests.Systems.Api.Services.Auth;

public class TestCreatePrincipal
{
    private IUnitOfWork _unitOfWork;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _authService = new AuthService(_unitOfWork, null!);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
    }

    [Test]
    public void WhenClaimsAreValid_ReturnsNewPrincipal()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var originalPrincipal = new ClaimsPrincipal(identity);

        // Act
        var result = _authService.CreatePrincipal(originalPrincipal);
        var newPrincipal = result.Value;

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(newPrincipal.HasClaim(ClaimTypes.NameIdentifier, "123"));
        Assert.That(newPrincipal.HasClaim(ClaimTypes.Email, "user@example.com"));
        Assert.That(newPrincipal.HasClaim(ClaimTypes.Role, "Admin"));
    }

    [TestCase(null, "user@example.com", "Admin")]
    [TestCase("123", null, "Admin")]
    [TestCase("123", "user@example.com", null)]
    public void WhenAnyClaimIsMissing_ReturnsFailure(string? userId, string? email, string? role)
    {
        // Arrange
        var claims = new List<Claim>();

        if (userId != null)
            claims.Add(new(ClaimTypes.NameIdentifier, userId));
        if (email != null)
            claims.Add(new(ClaimTypes.Email, email));
        if (role != null)
            claims.Add(new(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = _authService.CreatePrincipal(principal);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<MissingUserClaimError>());
    }
}
