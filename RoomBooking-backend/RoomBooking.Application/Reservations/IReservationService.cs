using RoomBooking.Application.Reservations.Results;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Application.Reservations;

public interface IReservationService
{
    Task<GetAllReservationsResult> GetAllReservationsAsync(int? roomId, int? organizerId, ReservationStatus? status,
        DateTime? from, DateTime? to);

    Task<CreateReservationResult> CreateReservationAsync(
        int roomId,
        int organizerId,
        string meetingTitle,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<int> attendeeIds);

    Task<CancelReservationResult> CancelReservationAsync(int id);
    Task<EndReservationEarlyResult> EndReservationEarlyAsync(int id);
}
