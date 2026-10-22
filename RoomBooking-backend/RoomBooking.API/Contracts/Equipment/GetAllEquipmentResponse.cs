using RoomBooking.API.Contracts;

namespace RoomBooking.API.Contracts.Equipment;

public class GetAllEquipmentResponse
{
    public ErrorResponse? Error { get; }
    public List<EquipmentDetailsResponse>? Equipment { get; }

    public GetAllEquipmentResponse(ErrorResponse? error, List<EquipmentDetailsResponse>? equipment)
    {
        Error = error;
        Equipment = equipment;
    }
}

public class EquipmentDetailsResponse
{
    public int Id { get; }
    public string Name { get; }

    public EquipmentDetailsResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}