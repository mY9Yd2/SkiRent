using System.Security.Claims;

using AutoFixture;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using SkiRent.Api.Authorization.Handlers;
using SkiRent.Api.Authorization.Requirements;

using SkiRent.Api.Data.Auth;

namespace SkiRent.UnitTests.Systems.Api.Authorization.Handlers;

public class TestSelfOrAdminAccessHandler
{
    private Fixture _fixture;
    private SelfOrAdminAccessHandler _handler;
    private SelfOrAdminAccessRequirement _requirement;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _handler = new SelfOrAdminAccessHandler();
        _requirement = new SelfOrAdminAccessRequirement();
    }

    private AuthorizationHandlerContext CreateContext(string role, string userId, object? routeUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var user = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();

        httpContext.Request.RouteValues.Returns(new RouteValueDictionary
        {
            ["userId"] = routeUserId
        });

        return new AuthorizationHandlerContext([_requirement], user, httpContext);
    }

    [Test]
    public async Task Admin_Always_Succeeds()
    {
        // Arrange
        var context = CreateContext(Roles.Admin, _fixture.Create<int>().ToString(), routeUserId: null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
        Assert.That(context.HasFailed, Is.False);
    }

    [Test]
    public async Task Customer_WithMatchingUserId_Succeeds()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var context = CreateContext(Roles.Customer, userId.ToString(), routeUserId: userId.ToString());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
        Assert.That(context.HasFailed, Is.False);
    }

    [Test]
    public async Task Customer_WithNonMatchingUserId_Fails()
    {
        // Arrange
        var context = CreateContext(Roles.Customer, "123", routeUserId: "456");

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task Customer_WithMissingRouteUserId_Fails()
    {
        // Arrange
        var context = CreateContext(Roles.Customer, "123", routeUserId: null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task Customer_WithInvalidRouteUserId_Fails()
    {
        // Arrange
        var context = CreateContext(Roles.Customer, "123", routeUserId: "not-a-number");

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task UnknownRole_Fails()
    {
        // Arrange
        var context = CreateContext("UnknownRole", "123", routeUserId: "123");

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task InvalidResourceType_Fails()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "123"),
            new(ClaimTypes.Role, Roles.Customer)
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext([_requirement], user, resource: new object());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasFailed, Is.True);
    }
}
