using RoomBooking.Application.Reservations.Results;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Application.Reservations;

public class ReservationService : IReservationService
{
    private static readonly SemaphoreSlim ReservationCreationLock = new(1, 1);
    
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    public async Task<GetAllReservationsResult> GetAllReservationsAsync(int? roomId, int? organizerId,
        ReservationStatus? status, DateTime? from, DateTime? to)
    {
        await _reservationRepository.CompleteExpiredReservationsAsync(DateTime.UtcNow);

        if (roomId != null)
        {
            var roomExist = await _roomRepository.RoomExistsAsync(roomId.Value);
            
            if (roomExist == null)
                return new GetAllReservationsResult(
                    new ReservationError("ROOM_NOT_FOUND", "No room has been found by the specified id."), null);
        }

        if (organizerId != null)
        {
            var userExist = await _userRepository.UserExistsAsync(organizerId.Value);
            
            if (userExist == null)
                return new GetAllReservationsResult(
                    new ReservationError("USER_NOT_FOUND", "No organizer has been found by the specified id."), null);
        }
        
        DateTime? toExclusiveUtc = null;
        if (to != null)
            toExclusiveUtc = to.Value.AddDays(1);

        var reservations = await _reservationRepository.GetReservationsAsync(
            roomId, organizerId, status, from, toExclusiveUtc);
        var now = DateTime.UtcNow;
        var reservationsWithActions = reservations
            .Select(reservation => (
                Reservation: reservation,
                AttendeeCount: reservation.Attendees.Count,
                CanBeCancelled: reservation.Status == ReservationStatus.Booked,
                CanBeEndedEarly: reservation.Status == ReservationStatus.Booked && reservation.StartTime <= now && now < reservation.EndTime))
            .ToList();

        return new GetAllReservationsResult(null, reservationsWithActions);
    }

    public async Task<CreateReservationResult> CreateReservationAsync(
        int roomId,
        int organizerId,
        string meetingTitle,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<int> attendeeIds)
    {
        await ReservationCreationLock.WaitAsync();

        try
        {
            var startUtc = startTime;
            var endUtc = endTime;

            var room = await _roomRepository.RoomExistsAsync(roomId);
            if (room == null)
                return new CreateReservationResult(new ReservationError("ROOM_NOT_FOUND",
                    "No room has been found by the specified id."));
            if (room.RoomStatus != RoomStatus.Active)
                return new CreateReservationResult(new ReservationError("ROOM_INACTIVE",
                    "Reservations cannot be made in an inactive room."));

            var organizer = await _userRepository.UserExistsAsync(organizerId);
            if (organizer == null)
                return new CreateReservationResult(new ReservationError("USER_NOT_FOUND",
                    "No organizer has been found by the specified id."));

            if (await _reservationRepository.HasOrganizerReservationOverlapAsync(
                    organizerId, startUtc, endUtc, null))
                return new CreateReservationResult(new ReservationError("ORGANIZER_BOOKING_CONFLICT",
                    "The organizer already has a booked reservation during this time."));

            var attendees = await _userRepository.GetUsersForReservationAsync(attendeeIds);
            if (attendees.Count != attendeeIds.Count)
                return new CreateReservationResult(new ReservationError("USER_NOT_FOUND",
                    "One or more attendee identifiers do not exist."));


            if (attendeeIds.Count > room.Capacity)
                return new CreateReservationResult(new ReservationError("ROOM_CAPACITY_EXCEEDED",
                    $"The room capacity is {room.Capacity}, but {attendeeIds.Count} attendees were provided."));

            if (await _reservationRepository.HasRoomReservationOverlapAsync(
                    roomId, startUtc, endUtc, null))
                return new CreateReservationResult(new ReservationError("ROOM_BOOKING_CONFLICT",
                    "The room already has a booked reservation during this time."));

            var reservation = new Reservation
            {
                RoomId = room.Id,
                MeetingRoom = room,
                OrganizerId = organizer.Id,
                Organizer = organizer,
                MeetingTitle = meetingTitle.Trim(),
                StartTime = startUtc,
                EndTime = endUtc,
                Status = ReservationStatus.Booked,
                Attendees = attendees
            };

            await _reservationRepository.AddReservationAsync(reservation);
            return new CreateReservationResult(null, reservation, reservation.Attendees.Count);
        }
        finally
        {
            ReservationCreationLock.Release();
        }
    }

    public async Task<CancelReservationResult> CancelReservationAsync(int id)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(id);
        
        if (reservation == null)
            return new CancelReservationResult(new ReservationError("RESERVATION_NOT_FOUND", "No reservation has been found by the specified id."));
        
        if (reservation.Status != ReservationStatus.Booked)
            return new CancelReservationResult(new ReservationError("RESERVATION_NOT_BOOKED",
                "Only booked reservations can be cancelled."));

        reservation.Status = ReservationStatus.Cancelled;
        
        await _reservationRepository.SaveReservationChangesAsync();
        
        return new CancelReservationResult(null, reservation, reservation.Attendees.Count);
    }

    public async Task<EndReservationEarlyResult> EndReservationEarlyAsync(int id)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(id);
        
        if (reservation == null)
            return new EndReservationEarlyResult(new ReservationError(
                "RESERVATION_NOT_FOUND", "No reservation has been found by the specified id."));
        
        if (reservation.Status != ReservationStatus.Booked)
            return new EndReservationEarlyResult(new ReservationError("RESERVATION_NOT_BOOKED",
                "Only booked reservations can be ended early."));

        var now = DateTime.UtcNow;
        
        if (reservation.StartTime >= now)
            return new EndReservationEarlyResult(new ReservationError("RESERVATION_NOT_STARTED",
                "Reservation can only be ended after its scheduled start time."));
        
        if (now >= reservation.EndTime)
            return new EndReservationEarlyResult(new ReservationError("RESERVATION_ALREADY_ENDED",
                "The scheduled reservation end time has already passed."));

        reservation.Status = ReservationStatus.Completed;
        reservation.ActualEndTime = now;
        
        await _reservationRepository.SaveReservationChangesAsync();
        return new EndReservationEarlyResult(null, reservation, reservation.Attendees.Count);
    }
}
