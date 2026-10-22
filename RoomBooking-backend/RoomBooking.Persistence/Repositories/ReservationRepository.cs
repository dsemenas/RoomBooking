using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Reservations;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetReservationsAsync(int? roomId, int? organizerId,
        ReservationStatus? status, DateTime? fromUtc, DateTime? toExclusiveUtc)
    {
        var query = _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.Organizer)
            .Include(r => r.Attendees)
            .AsNoTracking();

        if (roomId != null)
            query = query.Where(r => r.RoomId == roomId);
        if (organizerId != null)
            query = query.Where(r => r.OrganizerId == organizerId);
        if (status != null)
            query = query.Where(r => r.Status == status);
        if (fromUtc != null)
            query = query.Where(r => r.EndTime > fromUtc);
        if (toExclusiveUtc != null)
            query = query.Where(r => r.StartTime < toExclusiveUtc);

        return await query.OrderBy(r => r.StartTime).ThenBy(r => r.Id).ToListAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int id) =>
        await _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.Organizer)
            .Include(r => r.Attendees)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddReservationAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public Task SaveReservationChangesAsync() => _context.SaveChangesAsync();

    public async Task CompleteExpiredReservationsAsync(DateTime now)
    {
        var expiredReservations = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Booked && r.EndTime <= now)
            .ToListAsync();

        foreach (var reservation in expiredReservations)
        {
            reservation.Status = ReservationStatus.Completed;
            reservation.ActualEndTime ??= reservation.EndTime;
        }

        if (expiredReservations.Count > 0)
            await _context.SaveChangesAsync();
    }

    public Task<List<Reservation>> GetClosedReservationsAsync(int roomId) =>
        _context.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.RoomId == roomId && reservation.Status != ReservationStatus.Booked)
            .ToListAsync();

    public Task<bool> HasRoomReservationOverlapAsync(int roomId, DateTime start, DateTime end,
        int? excludeReservationId) =>
        _context.Reservations.AnyAsync(r =>
            r.RoomId == roomId &&
            r.Status == ReservationStatus.Booked &&
            r.StartTime < end &&
            r.EndTime > start &&
            (!excludeReservationId.HasValue || r.Id != excludeReservationId.Value));

    public Task<bool> HasOrganizerReservationOverlapAsync(int organizerId, DateTime start, DateTime end,
        int? excludeReservationId) =>
        _context.Reservations.AnyAsync(r =>
            r.OrganizerId == organizerId &&
            r.Status == ReservationStatus.Booked &&
            r.StartTime < end &&
            r.EndTime > start &&
            (!excludeReservationId.HasValue || r.Id != excludeReservationId.Value));
}
