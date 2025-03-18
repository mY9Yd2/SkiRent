using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.EquipmentImages;
using SkiRent.Shared.Contracts.EquipmentImages;

namespace SkiRent.Api.Controllers;

[Route("api/equipment-images")]
[ApiController]
public class EquipmentImagesController : BaseController
{
    private readonly IEquipmentImageService _equipmentImageService;

    public EquipmentImagesController(IEquipmentImageService equipmentImageService)
    {
        _equipmentImageService = equipmentImageService;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CreatedEquipmentImageResponse>> Create(
        [FromServices] IValidator<IFormFile> validator, IFormFile formFile)
    {
        var validationResult = await ValidateRequestAsync(validator, formFile);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentImageService.CreateAsync(formFile);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return CreatedAtAction(null, new { imageId = result.Value.Id }, result.Value);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IEnumerable<GetAllEquipmentImageResponse>>> GetAll()
    {
        var result = await _equipmentImageService.GetAllAsync();

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpPut("{imageId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<GetEquipmentImageResponse>> Update(
        [FromServices] IValidator<UpdateEquipmentImageRequest> validator, [FromRoute] Guid imageId, UpdateEquipmentImageRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentImageService.UpdateAsync(imageId, request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{imageId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete([FromRoute] Guid imageId)
    {
        var result = await _equipmentImageService.DeleteAsync(imageId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok();
    }
}
