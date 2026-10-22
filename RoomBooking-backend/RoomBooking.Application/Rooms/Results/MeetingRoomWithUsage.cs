using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Rooms.Results;

public class MeetingRoomWithUsage : MeetingRoom
{
    public bool IsOftenBookedAndRarelyUsed { get; set; }

    public MeetingRoomWithUsage(MeetingRoom room, bool isOftenBookedAndRarelyUsed)
    {
        Id = room.Id;
        Name = room.Name;
        Capacity = room.Capacity;
        Floor = room.Floor;
        RoomStatus = room.RoomStatus;
        Equipment = room.Equipment;
        Reservations = room.Reservations;
        IsOftenBookedAndRarelyUsed = isOftenBookedAndRarelyUsed;
    }
}
