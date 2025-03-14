using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Controllers;

[Route("api/equipment-categories")]
[ApiController]
public class EquipmentCategoriesController : BaseController
{
    private readonly IEquipmentCategoryService _equipmentCategoryService;

    public EquipmentCategoriesController(IEquipmentCategoryService equipmentCategoryService)
    {
        _equipmentCategoryService = equipmentCategoryService;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CreatedEquipmentCategoryResponse>> Create(
        [FromServices] IValidator<CreateEquipmentCategoryRequest> validator, [FromBody] CreateEquipmentCategoryRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentCategoryService.CreateAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return CreatedAtAction(null, new { equipmentCategoryId = result.Value.Id }, result.Value);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GetAllEquipmentCategoryResponse>>> GetAll()
    {
        var result = await _equipmentCategoryService.GetAllAsync();

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpPut("{categoryId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<GetEquipmentCategoryResponse>> Update(
        [FromServices] IValidator<UpdateEquipmentCategoryRequest> validator, [FromRoute] int categoryId, [FromBody] UpdateEquipmentCategoryRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentCategoryService.UpdateAsync(categoryId, request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{categoryId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete([FromRoute] int categoryId)
    {
        var result = await _equipmentCategoryService.DeleteAsync(categoryId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok();
    }
}
