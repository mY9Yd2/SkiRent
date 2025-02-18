using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkiRent.Api.Controllers.Base;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Services.Equipments;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Api.Controllers;

[ApiController]
[Route("api/equipments")]
public class EquipmentsController : BaseController
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentsController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CreatedEquipmentResponse>> Create(
        [FromServices] IValidator<CreateEquipmentRequest> validator, [FromBody] CreateEquipmentRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentService.CreateAsync(request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return CreatedAtAction(nameof(Get), new { equipmentId = result.Value.Id }, result.Value);
    }

    [HttpGet("{equipmentId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetEquipmentResponse>> Get([FromRoute] int equipmentId)
    {
        var result = await _equipmentService.GetAsync(equipmentId);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GetAllEquipmentResponse>>> GetAll()
    {
        var result = await _equipmentService.GetAllAsync();

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }

    [HttpPut("{equipmentId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<GetEquipmentResponse>> Update(
        [FromServices] IValidator<UpdateEquipmentRequest> validator, [FromRoute] int equipmentId, [FromBody] UpdateEquipmentRequest request)
    {
        var validationResult = await ValidateRequestAsync(validator, request);

        if (validationResult is not null)
        {
            return ValidationProblem(validationResult);
        }

        var result = await _equipmentService.UpdateAsync(equipmentId, request);

        if (result.IsFailed)
        {
            return Problem(result.Errors[0]);
        }

        return Ok(result.Value);
    }
}
