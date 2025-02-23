using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Services.Payments;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.Api.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsController : BaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromBody] PaymentResult paymentResult)
    {
        var result = await _paymentService.ProcessPaymentCallbackAsync(paymentResult);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok();
    }
}
