using System.Globalization;
using System.Reflection;

using AutoFixture;

using QuestPDF.Infrastructure;

using SkiRent.Api.Services.Payments;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.UnitTests.Utils;

namespace SkiRent.UnitTests.Systems.Services.Payments
{
    public class TestGenerateInvoiceDocument
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task GenerateInvoiceDocument()
        {
            // Arrange
            var request = new CreateInvoiceRequest
            {
                PaymentId = Guid.Empty,
                PersonalDetails = TestDataHelper.CreatePersonalDetails(_fixture),
                Items = [new() { Name = "Foobar-Item", SubText = "Lorem", TotalPrice = 1000 }],
                StartDate = new DateOnly(2025, 03, 02),
                EndDate = new DateOnly(2025, 03, 22),
                MerchantName = "SkiRent",
                Culture = CultureInfo.CreateSpecificCulture("hu-HU")
            };
            var paidAt = new DateTimeOffset(2025, 03, 02, 14, 15, 30, TimeSpan.Zero);

            var method = typeof(PaymentService).GetMethod("GenerateInvoiceDocument", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null);

            // Act
            var result = (IDocument?)method.Invoke(null, [request, paidAt]);

            // Assert
            Assert.That(result, Is.Not.Null);
            await Verify(result);
        }
    }
}
