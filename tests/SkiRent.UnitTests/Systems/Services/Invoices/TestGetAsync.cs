using AutoFixture;

using Microsoft.Extensions.Options;

using NSubstitute;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Invoices;

namespace SkiRent.UnitTests.Systems.Services.Invoices;

public class TestGetAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IInvoiceService _invoiceService;
    private IOptions<AppSettings> _appSettings;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _appSettings = Substitute.For<IOptions<AppSettings>>();

        _appSettings.Value.Returns(new AppSettings
        {
            BaseUrl = new Uri("http://localhost"),
            DataDirectoryPath = string.Empty,
            MerchantName = string.Empty,
        });

        _invoiceService = new InvoiceService(_unitOfWork, null!, _appSettings);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
    }

    [Test]
    public async Task WhenInvoiceNotFound_ReturnsFailedResult()
    {
        // Arrange
        var invoiceId = _fixture.Create<Guid>();
        var userId = _fixture.Create<int>();

        _unitOfWork.Invoices
            .GetByIdAsync(invoiceId)
            .Returns((Invoice?)null);

        // Act
        var result = await _invoiceService.GetAsync(invoiceId, userId, null!);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<InvoiceNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("invoiceId"), Is.EqualTo(invoiceId));
    }

    [Test]
    public async Task WhenInvoiceAccessDenied_ReturnsFailedResult()
    {
        // Arrange
        var invoice = _fixture.Create<Invoice>();
        var userId = _fixture.Create<int>();
        Func<string, bool> isInRole = _ => false;

        _unitOfWork.Invoices
            .GetByIdAsync(invoice.Id)
            .Returns(invoice);

        // Act
        var result = await _invoiceService.GetAsync(invoice.Id, userId, isInRole);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<InvoiceAccessDeniedError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("invoiceId"), Is.EqualTo(invoice.Id));
    }
}
