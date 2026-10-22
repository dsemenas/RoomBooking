using EquipmentEntity = RoomBooking.Domain.Entities.Equipment;

namespace RoomBooking.Application.Equipment.Results;

public class GetAllEquipmentResult
{
    public string? Error { get; }
    public string? ErrorDescription { get; }
    public List<EquipmentEntity>? Equipment { get; }

    public GetAllEquipmentResult(string? error, string? errorDescription, List<EquipmentEntity>? equipment)
    {
        Error = error;
        ErrorDescription = errorDescription;
        Equipment = equipment;
    }
}