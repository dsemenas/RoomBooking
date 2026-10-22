namespace RoomBooking.API.Contracts.Reservations;

using RoomBooking.API.Contracts;

public class ReservationResponse
{
    public ErrorResponse? Error { get; }
    public ReservationDetailsResponse? Reservation { get; }

    public ReservationResponse(ErrorResponse? error, ReservationDetailsResponse? reservation)
    {
        Error = error;
        Reservation = reservation;
    }
}