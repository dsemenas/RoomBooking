using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Rooms;

public interface IRoomRepository
{
    public Task<List<MeetingRoom>> GetAllRoomsAsync(int? minCapacity, List<int> equipmentIds, int? floor);
    public Task<MeetingRoom?> GetRoomByIdAsync(int id);
    public Task<MeetingRoom?> RoomExistsAsync(int id);
}
