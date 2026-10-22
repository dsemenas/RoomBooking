using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Reservations.Results;

public class CreateReservationResult
{
    public ReservationError? Error { get; }
    public Reservation? Reservation { get; }
    public int AttendeeCount { get; }

    public CreateReservationResult(ReservationError? error, Reservation? reservation = null, int attendeeCount = 0)
    {
        Error = error;
        Reservation = reservation;
        AttendeeCount = attendeeCount;
    }
}
