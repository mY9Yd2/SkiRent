using Microsoft.AspNetCore.Mvc;

using SkiRent.FakePay.Services.Payments;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.FakePay.Controllers;

[Route("/api/payments")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePaymentRequest request)
    {
        var paymentId = await _paymentService.CreateAsync(request);

        return Ok(paymentId);
    }
}
