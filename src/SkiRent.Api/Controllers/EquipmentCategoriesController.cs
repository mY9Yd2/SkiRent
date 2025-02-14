using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
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

    [HttpGet]
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
