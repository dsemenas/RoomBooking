using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Reservations.Results;

public class EndReservationEarlyResult
{
    public ReservationError? Error { get; }
    public Reservation? Reservation { get; }
    public int AttendeeCount { get; }

    public EndReservationEarlyResult(ReservationError? error, Reservation? reservation = null, int attendeeCount = 0)
    {
        Error = error;
        Reservation = reservation;
        AttendeeCount = attendeeCount;
    }
}
