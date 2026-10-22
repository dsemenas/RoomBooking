using RoomBooking.Domain.Helpers;

namespace RoomBooking.Domain.Entities;

public class MeetingRoom
{
    public int Id { get; set; }
    
    public string Name { get; set; } // in context of requirements this is treated as Name
    
    public int Capacity { get; set; }
    
    public int Floor { get; set; }
    
    public RoomStatus RoomStatus { get; set; }
    
    public List<Equipment> Equipment { get; set; }
    
    public List<Reservation> Reservations { get; set; }
}