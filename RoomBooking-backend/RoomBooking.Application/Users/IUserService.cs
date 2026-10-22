using RoomBooking.Application.Users.Results;

namespace RoomBooking.Application.Users;

public interface IUserService
{
    Task<GetAllUsersResult> GetAllUsersAsync();
}
