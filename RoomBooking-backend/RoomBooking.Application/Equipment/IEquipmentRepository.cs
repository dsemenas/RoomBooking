using EquipmentEntity = RoomBooking.Domain.Entities.Equipment;

namespace RoomBooking.Application.Equipment;

public interface IEquipmentRepository
{
    Task<List<EquipmentEntity>> GetAllEquipmentAsync();
    Task<bool> EquipmentExistsAsync(List<int> equipmentIds);
}
