using RoomBooking.Application.Rooms.Results;
using RoomBooking.Application.Equipment;
using RoomBooking.Application.Reservations;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;
using Microsoft.Extensions.Configuration;

namespace RoomBooking.Application.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly int _bookingCountThreshold;
    private readonly int _usageRateThresholdPercentage;
    
    public RoomService(
        IRoomRepository roomRepository,
        IEquipmentRepository equipmentRepository,
        IReservationRepository reservationRepository,
        IConfiguration configuration)
    {
        _roomRepository = roomRepository;
        _equipmentRepository = equipmentRepository;
        _reservationRepository = reservationRepository;
        var roomUsageRules = configuration.GetSection("RoomUsageRules");
        _bookingCountThreshold = int.Parse(roomUsageRules["BookingCountThreshold"]!);
        _usageRateThresholdPercentage = int.Parse(roomUsageRules["UsageRateThresholdPercentage"]!);
    }


    public async Task<GetAllRoomsResult> GetAllRoomsAsync(int? minCapacity, List<int> equipmentIds, int? floor)
    {
        var distinctEquipmentIds = equipmentIds.Distinct().ToList();
        if (distinctEquipmentIds.Count > 0 &&
            !await _equipmentRepository.EquipmentExistsAsync(distinctEquipmentIds))
            return new GetAllRoomsResult(
                "EQUIPMENT_NOT_FOUND",
                "Equipment contains one or more ids that do not exist.",
                null);

        var rooms = await _roomRepository.GetAllRoomsAsync(minCapacity, distinctEquipmentIds, floor);
        var roomsWithUsage = new List<MeetingRoomWithUsage>();
        foreach (var room in rooms)
        {
            var closedReservations = await _reservationRepository.GetClosedReservationsAsync(room.Id);
            var closedReservationsTimeInMinutes = 0;
            var closedReservationsActuallyUsedTimeInMinutes = 0;
            foreach (var reservation in closedReservations)
            {
                closedReservationsTimeInMinutes += GetClosedReservationTimeInMinutes(reservation);
                closedReservationsActuallyUsedTimeInMinutes +=
                    GetClosedReservationActuallyUsedTimeInMinutes(reservation);
            }
            var usageRate = CalculateUsageRate(
                closedReservationsTimeInMinutes,
                closedReservationsActuallyUsedTimeInMinutes);

            roomsWithUsage.Add(new MeetingRoomWithUsage(
                room,
            IsOftenBookedAndRarelyUsed(closedReservations.Count, usageRate)));
        }

        return new GetAllRoomsResult(null, null, roomsWithUsage);
    }

    public async Task<GetReservationDetailsResult> GetRoomDetailsAsync(int id)
    {
        var room = await _roomRepository.GetRoomByIdAsync(id);
        if (room == null)
            return new GetReservationDetailsResult(
                new RoomError("ROOM_NOT_FOUND", "No room has been found by the specified id."));

        var closedReservations = await _reservationRepository.GetClosedReservationsAsync(id);
        var closedReservationsTimeInMinutes = 0;
        var closedReservationsActuallyUsedTimeInMinutes = 0;
        foreach (var reservation in closedReservations)
        {
            closedReservationsTimeInMinutes += GetClosedReservationTimeInMinutes(reservation);
            closedReservationsActuallyUsedTimeInMinutes +=
                GetClosedReservationActuallyUsedTimeInMinutes(reservation);
        }
        var usageRate = CalculateUsageRate(
            closedReservationsTimeInMinutes,
            closedReservationsActuallyUsedTimeInMinutes);

        return new GetReservationDetailsResult(
            null,
            room,
            closedReservations.Count,
            closedReservationsTimeInMinutes,
            closedReservationsActuallyUsedTimeInMinutes,
            usageRate,
            IsOftenBookedAndRarelyUsed(closedReservations.Count, usageRate));
    }

    private bool IsOftenBookedAndRarelyUsed(int bookingCount, int usageRate) =>
        bookingCount > _bookingCountThreshold && usageRate < _usageRateThresholdPercentage;

    private static int CalculateUsageRate(
        int closedReservationsTimeInMinutes,
        int closedReservationsActuallyUsedTimeInMinutes) =>
        closedReservationsTimeInMinutes == 0
            ? 0
            : (int)Math.Round(
                closedReservationsActuallyUsedTimeInMinutes * 100d /
                closedReservationsTimeInMinutes);

    private static int GetClosedReservationTimeInMinutes(Reservation reservation) =>
        GetDurationInMinutes(reservation.EndTime - reservation.StartTime);

    private static int GetClosedReservationActuallyUsedTimeInMinutes(Reservation reservation)
    {
        if (reservation.Status == ReservationStatus.Cancelled)
            return 0;

        var actualEndTime = reservation.ActualEndTime ?? reservation.EndTime;
        return GetDurationInMinutes(actualEndTime - reservation.StartTime);
    }

    private static int GetDurationInMinutes(TimeSpan duration) => Math.Max(0, (int)duration.TotalMinutes);
}
