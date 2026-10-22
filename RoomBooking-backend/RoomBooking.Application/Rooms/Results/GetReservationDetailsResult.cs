using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Rooms.Results;

public class GetReservationDetailsResult
{
    public RoomError? Error { get; }
    public MeetingRoom? RoomDetails { get; }
    public int ClosedReservationsCount { get; }
    public int ClosedReservationsTimeInMinutes { get; }
    public int ClosedReservationsActuallyUsedTimeInMinutes { get; }
    public int UsageRate { get; }
    public bool IsOftenBookedAndRarelyUsed { get; }

    public GetReservationDetailsResult(
        RoomError? error,
        MeetingRoom? roomDetails = null,
        int closedReservationsCount = 0,
        int closedReservationsTimeInMinutes = 0,
        int closedReservationsActuallyUsedTimeInMinutes = 0,
        int usageRate = 0,
        bool isOftenBookedAndRarelyUsed = false)
    {
        Error = error;
        RoomDetails = roomDetails;
        ClosedReservationsCount = closedReservationsCount;
        ClosedReservationsTimeInMinutes = closedReservationsTimeInMinutes;
        ClosedReservationsActuallyUsedTimeInMinutes = closedReservationsActuallyUsedTimeInMinutes;
        UsageRate = usageRate;
        IsOftenBookedAndRarelyUsed = isOftenBookedAndRarelyUsed;
    }
}
