namespace RoomBooking.API.Contracts.Rooms;

public class GetAllRoomsResponse
{
    public ErrorResponse? Error { get;}

    public List<RoomDetails>? Rooms { get;}


    public GetAllRoomsResponse(ErrorResponse? error, List<RoomDetails>? rooms)
    {
        Error = error;
        Rooms = rooms;
    }
}