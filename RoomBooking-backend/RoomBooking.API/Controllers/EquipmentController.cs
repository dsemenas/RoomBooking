using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts;
using RoomBooking.API.Contracts.Equipment;
using RoomBooking.Application.Equipment;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllEquipmentResponse>> GetAllEquipmentAsync()
    {
        var result = await _equipmentService.GetAllEquipmentAsync();

        if (result.Error is not null)
            return BadRequest(new GetAllEquipmentResponse(
                new ErrorResponse(result.Error, result.ErrorDescription!), null));

        var equipment = (result.Equipment ?? []).Select(item =>
            new EquipmentDetailsResponse(item.Id, item.Name)).ToList();

        return Ok(new GetAllEquipmentResponse(null, equipment));
    }
}
