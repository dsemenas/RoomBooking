using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Reservations.Results;

public class GetAllReservationsResult
{
    public ReservationError? Error { get; }
    public List<(Reservation Reservation, int AttendeeCount, bool CanBeCancelled, bool CanBeEndedEarly)>? Reservations { get; }

    public GetAllReservationsResult(
        ReservationError? error,
        List<(Reservation Reservation, int AttendeeCount, bool CanBeCancelled, bool CanBeEndedEarly)>? reservations)
    {
        Error = error;
        Reservations = reservations;
    }
}
