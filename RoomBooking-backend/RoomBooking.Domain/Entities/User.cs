namespace RoomBooking.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public List<Reservation> OrganizedReservations { get; set; }
    
    public List <Reservation> AttendingReservations { get; set; }
}