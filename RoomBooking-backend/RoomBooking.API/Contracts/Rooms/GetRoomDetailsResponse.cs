namespace RoomBooking.API.Contracts.Rooms;

public class GetRoomDetailsResponse
{
    public ErrorResponse? Error { get; set; }
    
    public RoomDetailsWithStats? Room { get; set; }

    public GetRoomDetailsResponse(ErrorResponse? error, RoomDetailsWithStats? room)
    {
        Error = error;
        Room = room;
    }
}


public class RoomDetailsWithStats : RoomDetails
{
    public int ClosedReservationsCount { get; set; }
    
    public int ClosedReservationsTimeInMinutes { get; set; }
    
    public int ClosedReservationsActuallyUsedTimeInMinutes { get; set; }
    
    public int UsageRate { get; set; }
    
    public RoomDetailsWithStats(
        int id,
        string name,
        int capacity,
        int floor,
        List<string> equipment,
        string status,
        int closedReservationsCount,
        int closedReservationsTimeInMinutes,
        int closedReservationsActuallyUsedTimeInMinutes,
        int usageRate,
        bool isOftenBookedAndRarelyUsed)
        : base(id, name, capacity, floor, equipment, status, isOftenBookedAndRarelyUsed)
    {
        ClosedReservationsCount = closedReservationsCount;
        ClosedReservationsTimeInMinutes = closedReservationsTimeInMinutes;
        ClosedReservationsActuallyUsedTimeInMinutes = closedReservationsActuallyUsedTimeInMinutes;
        UsageRate = usageRate;
    }
}
