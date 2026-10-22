namespace RoomBooking.Application.Rooms.Results;

public class GetAllRoomsResult
{
    public string? Error { get; set; }
    
    public string? ErrorDescription { get; set; }
    
    public List<MeetingRoomWithUsage>? Rooms { get; set; }

    public GetAllRoomsResult(
        string? error,
        string? errorDescription,
        List<MeetingRoomWithUsage>? rooms)
    {
        Error = error;
        ErrorDescription = errorDescription;
        Rooms = rooms;
    }
    
}
