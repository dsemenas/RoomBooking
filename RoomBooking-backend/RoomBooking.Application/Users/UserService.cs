using RoomBooking.Application.Users.Results;
using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetAllUsersResult> GetAllUsersAsync()
    {
        List<User> users = await _userRepository.GetAllUsersAsync();

        return new GetAllUsersResult(null, null, users);
    }
}
