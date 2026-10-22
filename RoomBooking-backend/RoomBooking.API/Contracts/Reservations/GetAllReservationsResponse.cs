namespace RoomBooking.API.Contracts.Reservations;

using RoomBooking.API.Contracts;

public class GetAllReservationsResponse
{
    public ErrorResponse? Error { get; }
    public List<ReservationDetailsResponse>? Reservations { get; }

    public GetAllReservationsResponse(ErrorResponse? error, List<ReservationDetailsResponse>? reservations)
    {
        Error = error;
        Reservations = reservations;
    }
}