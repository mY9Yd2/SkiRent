using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Api.Controllers;

[ApiController]
[Route("api/equipment-categories")]
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
}
