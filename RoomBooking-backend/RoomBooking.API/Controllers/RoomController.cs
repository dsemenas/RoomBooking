using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts;
using RoomBooking.API.Contracts.Rooms;
using RoomBooking.Application;
using RoomBooking.Application.Rooms;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllRoomsResponse>> GetAllRoomsAsync(
        [FromQuery] int? minCapacity,
        [FromQuery] List<int>? equipment,
        [FromQuery] int? floor)
    {
        List<int> equipmentIds = equipment ?? [];

        List<string> errors = new List<string>();

        if (minCapacity < 1 || minCapacity > 100)
            errors.Add("Minimum capacity must be between 1 and 100.");

        if (floor < 0 || floor > 100)
            errors.Add("Floor must be between 0 and 100.");

        if (errors.Count > 0)
        {
            return BadRequest(new GetAllRoomsResponse(
                new ErrorResponse("INVALID_FILTER_VALUE", string.Join(", ", errors)), null));
        }

        var result = await _roomService.GetAllRoomsAsync(minCapacity, equipmentIds, floor);

        if (result.Error != null)
        {
            return BadRequest(new GetAllRoomsResponse(
                new ErrorResponse(result.Error, result.ErrorDescription!), null));
        }

        var rooms = result.Rooms.Select(r => new RoomDetails(
            r.Id,
            r.Name,
            r.Capacity,
            r.Floor,
            r.Equipment.Select(e => e.Name).ToList(),
            r.RoomStatus.ToString(),
            r.IsOftenBookedAndRarelyUsed
        )).ToList();

        return Ok(new GetAllRoomsResponse(null, rooms));
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetRoomDetailsResponse>> GetRoomDetailsAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new GetRoomDetailsResponse(
                new ErrorResponse("INVALID_ROOM_ID", "Room id must be a positive integer."), null));
        }

        var result = await _roomService.GetRoomDetailsAsync(id);
        if (result.Error != null)
            return NotFound(new GetRoomDetailsResponse(
                new ErrorResponse(result.Error.ErrorCode, result.Error.ErrorMessage), null));

        var room = result.RoomDetails!;
        var roomDetails = new RoomDetailsWithStats(
            room.Id,
            room.Name,
            room.Capacity,
            room.Floor,
            room.Equipment.Select(e => e.Name).ToList(),
            room.RoomStatus.ToString(),
            result.ClosedReservationsCount,
            result.ClosedReservationsTimeInMinutes,
            result.ClosedReservationsActuallyUsedTimeInMinutes,
            result.UsageRate,
            result.IsOftenBookedAndRarelyUsed);

        return Ok(new GetRoomDetailsResponse(null, roomDetails));
    }
}
