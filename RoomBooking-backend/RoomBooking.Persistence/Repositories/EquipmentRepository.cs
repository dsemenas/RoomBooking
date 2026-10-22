using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Equipment;
using RoomBooking.Domain.Entities;

namespace RoomBooking.Persistence.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _context;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Equipment>> GetAllEquipmentAsync() =>
        _context.Equipment.OrderBy(equipment => equipment.Id).ToListAsync();

    public async Task<bool> EquipmentExistsAsync(List<int> equipmentIds)
    {
        var found = await _context.Equipment.CountAsync(equipment => equipmentIds.Contains(equipment.Id));
        return found == equipmentIds.Count;
    }
}
