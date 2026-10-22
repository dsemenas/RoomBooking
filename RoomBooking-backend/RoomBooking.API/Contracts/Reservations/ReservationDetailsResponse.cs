namespace RoomBooking.API.Contracts.Reservations;

public class ReservationDetailsResponse
{
    public int Id { get; }
    public string RoomName { get; }
    public string OrganizerFullName { get; }
    public string MeetingTitle { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public int AttendeeCount { get; }
    public string Status { get; }
    
    public bool CanBeCancelled { get; }
    
    public bool CanBeEndedEarly { get; }

    public ReservationDetailsResponse(int id, string roomName, string organizerFullName, string meetingTitle,
        DateTime startTime, DateTime endTime, int attendeeCount, string status, bool canBeCancelled = false,
        bool canBeEndedEarly = false)
    {
        Id = id;
        RoomName = roomName;
        OrganizerFullName = organizerFullName;
        MeetingTitle = meetingTitle;
        StartTime = startTime;
        EndTime = endTime;
        AttendeeCount = attendeeCount;
        Status = status;
        CanBeCancelled = canBeCancelled;
        CanBeEndedEarly = canBeEndedEarly;
    }
}
