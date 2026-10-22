using RoomBooking.Application.Rooms.Results;

namespace RoomBooking.Application.Rooms;

public interface IRoomService
{
    public Task<GetAllRoomsResult> GetAllRoomsAsync(int? minCapacity, List<int> equipmentIds, int? floor);
    public Task<GetReservationDetailsResult> GetRoomDetailsAsync(int id);
}
