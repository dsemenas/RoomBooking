using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.OrganizedReservations.Where(r => r.Status == ReservationStatus.Booked))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }

    public Task<User?> UserExistsAsync(int id) =>
        _context.Users.FirstOrDefaultAsync(user => user.Id == id);

    public Task<List<User>> GetUsersForReservationAsync(IReadOnlyCollection<int> ids) =>
        _context.Users.Where(user => ids.Contains(user.Id)).ToListAsync();
}
