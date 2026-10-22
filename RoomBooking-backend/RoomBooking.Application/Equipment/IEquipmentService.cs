using RoomBooking.Application.Equipment.Results;

namespace RoomBooking.Application.Equipment;

public interface IEquipmentService
{
    Task<GetAllEquipmentResult> GetAllEquipmentAsync();
}
