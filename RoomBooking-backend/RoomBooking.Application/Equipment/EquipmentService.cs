using RoomBooking.Application.Equipment.Results;

namespace RoomBooking.Application.Equipment;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public EquipmentService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<GetAllEquipmentResult> GetAllEquipmentAsync()
    {
        var equipment = await _equipmentRepository.GetAllEquipmentAsync();
        return new GetAllEquipmentResult(null, null, equipment);
    }
}
