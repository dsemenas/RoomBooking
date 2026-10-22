using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Users;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> UserExistsAsync(int id);
    Task<List<User>> GetUsersForReservationAsync(IReadOnlyCollection<int> ids);
}
