using System.Net.Mime;
using System.Security.Claims;

using AutoFixture;

using FluentResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using SkiRent.Api.Controllers;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Exceptions;
using SkiRent.Api.Services.Invoices;

namespace SkiRent.UnitTests.Systems.Api.Controllers.Invoices;

public class TestGet
{
    private Fixture _fixture;
    private IInvoiceService _invoiceService;
    private InvoicesController _controller;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();

        _invoiceService = Substitute.For<IInvoiceService>();

        _controller = new InvoicesController(_invoiceService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private void SetUser(string userId, string role = Roles.Customer)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext.HttpContext.User = principal;
    }

    [Test]
    public async Task InvalidUserId_ReturnsUnauthorized()
    {
        // Arrange
        SetUser("not-an-int");
        var invoiceId = _fixture.Create<Guid>();

        // Act
        var result = await _controller.Get(invoiceId);

        // Assert
        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public void InvoiceRetrievalFails_ReturnsProblem()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser(userId.ToString());

        var invoiceId = _fixture.Create<Guid>();
        var errorMessage = _fixture.Create<string>();

        _invoiceService.GetAsync(invoiceId, userId, Arg.Any<Func<string, bool>>())
            .Returns(Result.Fail(errorMessage));

        // Act & Assert
        Assert.ThrowsAsync<UnhandledErrorException>(async () => await _controller.Get(invoiceId));
    }

    [Test]
    public async Task Success_ReturnsFileResult()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        SetUser(userId.ToString(), Roles.Admin);

        var invoiceId = _fixture.Create<Guid>();
        var fileBytes = _fixture.CreateMany<byte>(100).ToArray();

        _invoiceService.GetAsync(invoiceId, userId, Arg.Any<Func<string, bool>>())
            .Returns(Result.Ok(fileBytes));

        // Act
        var result = await _controller.Get(invoiceId);

        // Assert
        var fileResult = result as FileContentResult;
        Assert.That(fileResult, Is.Not.Null);
        Assert.That(fileResult!.ContentType, Is.EqualTo(MediaTypeNames.Application.Pdf));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo($"Számla_{invoiceId}.pdf"));
        Assert.That(fileResult.FileContents, Is.EqualTo(fileBytes));
    }
}
