namespace RoomBooking.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<MeetingRoom> MeetingRooms { get; set; } = [];
}