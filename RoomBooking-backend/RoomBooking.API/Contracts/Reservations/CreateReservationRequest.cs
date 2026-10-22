namespace RoomBooking.API.Contracts.Reservations;

public class CreateReservationRequest
{
    public int RoomId { get; set; }
    public int OrganizerId { get; set; }
    public string MeetingTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<int> AttendeeIds { get; set; } = [];
}