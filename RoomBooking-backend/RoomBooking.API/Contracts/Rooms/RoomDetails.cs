namespace RoomBooking.API.Contracts.Rooms;

public class RoomDetails
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public int Floor { get; set; }
    public List<string> Equipment { get; set; }
    public string Status { get; set; }
    public bool IsOftenBookedAndRarelyUsed { get; set; }

    public RoomDetails(
        int id,
        string name,
        int capacity,
        int floor,
        List<string> equipment,
        string status,
        bool isOftenBookedAndRarelyUsed = false)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
        Floor = floor;
        Equipment = equipment;
        Status = status;
        IsOftenBookedAndRarelyUsed = isOftenBookedAndRarelyUsed;
    }
}
