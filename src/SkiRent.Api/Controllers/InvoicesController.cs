using System.Net.Mime;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.Invoices;

namespace SkiRent.Api.Controllers;

[Route("api/invoices")]
[ApiController]
public class InvoicesController : BaseController
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("{invoiceId:guid}")]
    [Authorize(Policy = Policies.CustomerOrAdminAccess)]
    public async Task<IActionResult> Get([FromRoute] Guid invoiceId)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(nameIdentifier, out int userId))
        {
            return Unauthorized();
        }

        var result = await _invoiceService.GetAsync(invoiceId, userId, User.IsInRole);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return File(result.Value, MediaTypeNames.Application.Pdf, fileDownloadName: $"Számla_{invoiceId}.pdf");
    }
}
