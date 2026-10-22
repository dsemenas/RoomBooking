using RoomBooking.Domain.Helpers;

namespace RoomBooking.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    
    public int RoomId { get; set; }
    
    public MeetingRoom MeetingRoom { get; set; } = null!;
    
    public int OrganizerId { get; set; }
    
    public User Organizer { get; set; } = null!;
    
    public List<User> Attendees { get; set; } = [];
    
    public string MeetingTitle { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public DateTime? ActualEndTime { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Booked;

    public Reservation()
    {
    }

    public Reservation(int id, int roomId, int organizerId, string meetingTitle, DateTime startTime,
        DateTime endTime, DateTime? actualEndTime, ReservationStatus status)
    {
        Id = id;
        RoomId = roomId;
        OrganizerId = organizerId;
        MeetingTitle = meetingTitle;
        StartTime = startTime;
        EndTime = endTime;
        ActualEndTime = actualEndTime;
        Status = status;
    }
    
}