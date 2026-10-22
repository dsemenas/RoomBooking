using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts;
using RoomBooking.API.Contracts.Users;
using RoomBooking.Application.Users;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllUsersDetailsResponse>> GetAllUsersAsync()
    {
        var result = await _userService.GetAllUsersAsync();

        if (result.Error != null)
        {
            return BadRequest(new GetAllUsersDetailsResponse(
                new ErrorResponse(result.Error, result.ErrorDescription!), null));
        }

        var users = (result.Users ?? []).Select(u => new UserDetailsResponse(
            u.Id,
            u.FirstName,
            u.LastName,
            u.OrganizedReservations.Count
        )).ToList();

        return Ok(new GetAllUsersDetailsResponse(null, users));
    }
}
