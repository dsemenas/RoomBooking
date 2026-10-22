using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Application.Reservations;

public interface IReservationRepository
{
    Task<List<Reservation>> GetReservationsAsync(int? roomId, int? organizerId, ReservationStatus? status,
        DateTime? fromUtc, DateTime? toExclusiveUtc);

    Task<Reservation?> GetReservationByIdAsync(int id);
    Task<bool> HasRoomReservationOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeReservationId);
    Task<bool> HasOrganizerReservationOverlapAsync(int organizerId, DateTime start, DateTime end, int? excludeReservationId);
    Task AddReservationAsync(Reservation reservation);
    Task SaveReservationChangesAsync();
    
    Task CompleteExpiredReservationsAsync(DateTime now);
    Task<List<Reservation>> GetClosedReservationsAsync(int roomId);
}
