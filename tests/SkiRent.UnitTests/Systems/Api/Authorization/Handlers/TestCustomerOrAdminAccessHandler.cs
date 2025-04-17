using System.Security.Claims;

using AutoFixture;

using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Authorization.Handlers;
using SkiRent.Api.Authorization.Requirements;

using SkiRent.Api.Data.Auth;

namespace SkiRent.UnitTests.Systems.Api.Authorization.Handlers;

public class TestCustomerOrAdminAccessHandler
{
    private Fixture _fixture;
    private CustomerOrAdminAccessHandler _handler;
    private CustomerOrAdminAccessRequirement _requirement;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _handler = new CustomerOrAdminAccessHandler();
        _requirement = new CustomerOrAdminAccessRequirement();
    }

    private AuthorizationHandlerContext CreateContext(string role, string? userId = null)
    {
        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        return new AuthorizationHandlerContext([_requirement], principal, null);
    }

    [Test]
    public async Task AdminUser_Succeeds()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        var context = CreateContext(Roles.Admin, userId);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
        Assert.That(context.HasFailed, Is.False);
    }

    [Test]
    public async Task CustomerWithUserId_Succeeds()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        var context = CreateContext(Roles.Customer, userId);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
        Assert.That(context.HasFailed, Is.False);
    }

    [Test]
    public async Task CustomerWithoutUserId_Fails()
    {
        // Arrange
        var context = CreateContext(Roles.Customer, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task UnknownRole_Fails()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        var context = CreateContext("UnknownRole", userId);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task NoClaims_Fails()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext([_requirement], principal, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }
}
