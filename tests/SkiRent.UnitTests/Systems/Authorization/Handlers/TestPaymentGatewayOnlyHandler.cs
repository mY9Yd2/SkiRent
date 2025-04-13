using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using AutoFixture;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using NSubstitute;

using SkiRent.Api.Authorization.Handlers;

using SkiRent.Api.Authorization.Requirements;

using SkiRent.Api.Configurations;

namespace SkiRent.UnitTests.Systems.Authorization.Handlers;

public class TestPaymentGatewayOnlyHandler
{
    private Fixture _fixture;
    private PaymentGatewayOnlyHandler _handler;
    private PaymentGatewayOnlyRequirement _requirement;
    private IOptions<PaymentGatewayOptions> _options;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        var secret = _fixture.Create<string>();

        _options = Options.Create(new PaymentGatewayOptions
        {
            BaseUrl = _fixture.Create<Uri>(),
            SharedSecret = secret
        });

        _handler = new PaymentGatewayOnlyHandler(_options);
        _requirement = new PaymentGatewayOnlyRequirement();
    }

    private (AuthorizationHandlerContext context, HttpContext httpContext) CreateContextWithRequest(
        string requestBody, string sharedSecret, bool includeSignature = true, bool correctSignature = true)
    {
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
        var memoryStream = new MemoryStream(bodyBytes);

        var httpContext = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();

        request.Body.Returns(memoryStream);
        request.Headers.Returns(new HeaderDictionary());

        if (includeSignature)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(correctSignature ? requestBody : "wrong-body"));
            var encoded = Convert.ToBase64String(hash);
            request.Headers["X-Signature"] = new StringValues(encoded);
        }

        httpContext.Request.Returns(request);
        request.EnableBuffering();

        var principal = Substitute.For<ClaimsPrincipal>();
        var context = new AuthorizationHandlerContext([_requirement], principal, httpContext);

        return (context, httpContext);
    }

    [Test]
    public async Task ValidSignature_Succeeds()
    {
        // Arrange
        var requestBody = _fixture.Create<string>();
        var (context, _) = CreateContextWithRequest(requestBody, _options.Value.SharedSecret);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
        Assert.That(context.HasFailed, Is.False);
    }

    [Test]
    public async Task MissingSignatureHeader_Fails()
    {
        // Arrange
        var requestBody = _fixture.Create<string>();
        var (context, _) = CreateContextWithRequest(requestBody, _options.Value.SharedSecret, includeSignature: false);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task InvalidSignature_Fails()
    {
        // Arrange
        var requestBody = _fixture.Create<string>();
        var (context, _) = CreateContextWithRequest(requestBody, _options.Value.SharedSecret, correctSignature: false);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task EmptyBody_Fails()
    {
        // Arrange
        var (context, _) = CreateContextWithRequest(string.Empty, _options.Value.SharedSecret);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }

    [Test]
    public async Task InvalidResourceType_Fails()
    {
        // Arrange
        var principal = Substitute.For<ClaimsPrincipal>();
        var context = new AuthorizationHandlerContext([_requirement], principal, resource: new object());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
        Assert.That(context.HasFailed, Is.True);
    }
}
