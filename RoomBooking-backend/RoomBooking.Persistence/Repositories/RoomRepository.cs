using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Rooms;
using RoomBooking.Domain.Entities;

namespace RoomBooking.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;
    
    public  RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MeetingRoom>> GetAllRoomsAsync(int? minCapacity, List<int> equipmentIds, int? floor)
    {
        var query = _context.MeetingRooms.Include(r => r.Equipment).AsQueryable();

        if (minCapacity is not null)
            query = query.Where(r => r.Capacity >= minCapacity);

        if (floor is not null)
            query = query.Where(r => r.Floor == floor);

        if (equipmentIds.Count > 0)
            query = query.Where(r => r.Equipment.Count(e => equipmentIds.Contains(e.Id)) == equipmentIds.Count);

        return await query.OrderBy(r => r.Floor).ThenBy(r => r.Name).ToListAsync();
    }

    public Task<MeetingRoom?> GetRoomByIdAsync(int id) =>
        _context.MeetingRooms
            .Include(room => room.Equipment)
            .FirstOrDefaultAsync(room => room.Id == id);

    public Task<MeetingRoom?> RoomExistsAsync(int id) =>
        _context.MeetingRooms.FirstOrDefaultAsync(room => room.Id == id);
}
