using Moq;
using RoomBooking.Application.Reservations;
using RoomBooking.Application.Reservations.Results;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;
using Xunit;

namespace RoomBooking.Tests.Reservations;

public class ReservationServiceTests
{
    private readonly Mock<IReservationRepository> _reservationRepository = new();
    private readonly Mock<IRoomRepository> _roomRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _reservationRepository
            .Setup(repository => repository.CompleteExpiredReservationsAsync(It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        _service = new ReservationService(
            _reservationRepository.Object, _roomRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task GetAllReservations_WithUnknownRoom_ReturnsRoomNotFoundAsync()
    {
        _roomRepository.Setup(repository => repository.RoomExistsAsync(99)).ReturnsAsync((MeetingRoom?)null);

        var result = await _service.GetAllReservationsAsync(99, null, null, null, null);

        AssertError(result.Error, "ROOM_NOT_FOUND");
        Assert.Null(result.Reservations);
        _reservationRepository.Verify(repository => repository.GetReservationsAsync(
            It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<ReservationStatus?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Fact]
    public async Task GetAllReservations_WithUnknownOrganizer_ReturnsUserNotFoundAsync()
    {
        _roomRepository.Setup(repository => repository.RoomExistsAsync(1))
            .ReturnsAsync(new MeetingRoom { Id = 1 });
        _userRepository.Setup(repository => repository.UserExistsAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.GetAllReservationsAsync(1, 99, null, null, null);

        AssertError(result.Error, "USER_NOT_FOUND");
        Assert.Null(result.Reservations);
    }

    [Fact]
    public async Task GetAllReservations_PassesInclusiveEndDateAsExclusiveDateAsync()
    {
        var reservations = new List<Reservation>
        {
            new() { Status = ReservationStatus.Booked, StartTime = DateTime.UtcNow.AddHours(2) }
        };
        var to = new DateTime(2026, 10, 22);

        _roomRepository.Setup(repository => repository.RoomExistsAsync(1))
            .ReturnsAsync(new MeetingRoom { Id = 1 });
        _userRepository.Setup(repository => repository.UserExistsAsync(2))
            .ReturnsAsync(new User { Id = 2 });
        _reservationRepository.Setup(repository => repository.GetReservationsAsync(1, 2, ReservationStatus.Booked,
                new DateTime(2026, 10, 20), to.AddDays(1)))
            .ReturnsAsync(reservations);

        var result = await _service.GetAllReservationsAsync(
            1, 2, ReservationStatus.Booked, new DateTime(2026, 10, 20), to);

        Assert.Null(result.Error);
        Assert.NotNull(result.Reservations);
        Assert.Single(result.Reservations);
        Assert.True(result.Reservations[0].CanBeCancelled);
        _reservationRepository.Verify(repository => repository.GetReservationsAsync(
            1, 2, ReservationStatus.Booked, new DateTime(2026, 10, 20), to.AddDays(1)), Times.Once);
    }
    
    [Fact]
public async Task CreateReservationAsync_WhenCalledConcurrently_AllowsOnlyOneReservation()
{
    var room = new MeetingRoom
    {
        Id = 1,
        Capacity = 5,
        RoomStatus = RoomStatus.Active
    };

    var organizer = new User { Id = 2 };
    var reservations = new List<Reservation>();

    var firstSaveStarted = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);

    var allowFirstSave = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);

    var request = CreateRequest(room.Id, organizer.Id);

    _roomRepository
        .Setup(repository => repository.RoomExistsAsync(room.Id))
        .ReturnsAsync(room);

    _userRepository
        .Setup(repository => repository.UserExistsAsync(organizer.Id))
        .ReturnsAsync(organizer);

    _userRepository
        .Setup(repository => repository.GetUsersForReservationAsync(
            It.IsAny<IReadOnlyCollection<int>>()))
        .ReturnsAsync([]);

    _reservationRepository
        .Setup(repository => repository.HasOrganizerReservationOverlapAsync(
            organizer.Id,
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            null))
        .ReturnsAsync(false);

    _reservationRepository
        .Setup(repository => repository.HasRoomReservationOverlapAsync(
            room.Id,
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            null))
        .Returns(() =>
            Task.FromResult(reservations.Count > 0));

    _reservationRepository
        .Setup(repository => repository.AddReservationAsync(
            It.IsAny<Reservation>()))
        .Returns(async (Reservation reservation) =>
        {
            firstSaveStarted.TrySetResult(true);

            await allowFirstSave.Task;

            reservations.Add(reservation);
        });

    var firstRequest = CreateReservationAsync(request);

    // Ensure request 1 has entered the save operation.
    await firstSaveStarted.Task;

    // Request 2 starts while request 1 still owns the semaphore.
    var secondRequest = CreateReservationAsync(request);

    // Allow request 1 to finish and release the semaphore.
    allowFirstSave.SetResult(true);

    var results = await Task.WhenAll(firstRequest, secondRequest);

    Assert.Equal(1, results.Count(result => result.Error == null));
    Assert.Equal(1, results.Count(result =>
        result.Error?.ErrorCode == "ROOM_BOOKING_CONFLICT"));

    Assert.Single(reservations);
}

    [Fact]
    public async Task CreateReservation_WithValidRequest_TrimsTitleAndAddsBookedReservationAsync()
    {
        var room = new MeetingRoom { Id = 4, Capacity = 5, RoomStatus = RoomStatus.Active };
        var organizer = new User { Id = 7 };
        var attendees = new List<User> { new() { Id = 8 }, new() { Id = 9 } };
        var request = CreateRequest(room.Id, organizer.Id, attendees.Select(user => user.Id).ToList());
        Reservation? addedReservation = null;

        _roomRepository.Setup(repository => repository.RoomExistsAsync(room.Id)).ReturnsAsync(room);
        _userRepository.Setup(repository => repository.UserExistsAsync(organizer.Id)).ReturnsAsync(organizer);
        _userRepository.Setup(repository => repository.GetUsersForReservationAsync(request.AttendeeIds)).ReturnsAsync(attendees);
        _reservationRepository.Setup(repository => repository.AddReservationAsync(It.IsAny<Reservation>()))
            .Callback<Reservation>(reservation => addedReservation = reservation)
            .Returns(Task.CompletedTask);

        var result = await CreateReservationAsync(request);

        Assert.Null(result.Error);
        Assert.Same(addedReservation, result.Reservation);
        Assert.NotNull(addedReservation);
        Assert.Equal("Planning meeting", addedReservation.MeetingTitle);
        Assert.Equal(ReservationStatus.Booked, addedReservation.Status);
        Assert.Same(room, addedReservation.MeetingRoom);
        Assert.Same(organizer, addedReservation.Organizer);
        Assert.Equal(attendees, addedReservation.Attendees);
        _reservationRepository.Verify(repository => repository.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task CreateReservation_WithInactiveRoom_ReturnsRoomInactiveAsync()
    {
        _roomRepository.Setup(repository => repository.RoomExistsAsync(1))
            .ReturnsAsync(new MeetingRoom { Id = 1, Capacity = 5, RoomStatus = RoomStatus.Inactive });

        var result = await CreateReservationAsync(CreateRequest(1, 2));

        AssertError(result.Error, "ROOM_INACTIVE");
        _userRepository.Verify(repository => repository.UserExistsAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData("organizer", "ORGANIZER_BOOKING_CONFLICT")]
    [InlineData("attendee", "USER_NOT_FOUND")]
    [InlineData("room", "ROOM_BOOKING_CONFLICT")]
    public async Task CreateReservation_WhenBusinessRuleFails_ReturnsExpectedErrorAsync(
        string scenario, string expectedErrorCode)
    {
        var room = new MeetingRoom
        {
            Id = 1, Capacity = 5, RoomStatus = RoomStatus.Active
        };
        var organizer = new User { Id = 2 };
        var attendeeIds = new List<int> { 3 };

        _roomRepository.Setup(repository => repository.RoomExistsAsync(room.Id)).ReturnsAsync(room);
        _userRepository.Setup(repository => repository.UserExistsAsync(organizer.Id)).ReturnsAsync(organizer);
        _reservationRepository.Setup(repository => repository.HasOrganizerReservationOverlapAsync(
                organizer.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(scenario == "organizer");
        _userRepository.Setup(repository => repository.GetUsersForReservationAsync(attendeeIds))
            .ReturnsAsync(scenario == "attendee"
                ? new List<User>()
                : attendeeIds.Select(id => new User { Id = id }).ToList());
        _reservationRepository.Setup(repository => repository.HasRoomReservationOverlapAsync(
                room.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(scenario == "room");

        var result = await CreateReservationAsync(CreateRequest(room.Id, organizer.Id, attendeeIds));

        AssertError(result.Error, expectedErrorCode);
        _reservationRepository.Verify(repository => repository.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task CreateReservation_WhenAttendeesExceedRoomCapacity_ReturnsCapacityErrorAsync()
    {
        var room = new MeetingRoom { Id = 1, Capacity = 1, RoomStatus = RoomStatus.Active };
        var organizer = new User { Id = 2 };
        var attendeeIds = new List<int> { 3, 4 };
        var attendees = attendeeIds.Select(id => new User { Id = id }).ToList();

        _roomRepository.Setup(repository => repository.RoomExistsAsync(room.Id)).ReturnsAsync(room);
        _userRepository.Setup(repository => repository.UserExistsAsync(organizer.Id)).ReturnsAsync(organizer);
        _userRepository.Setup(repository => repository.GetUsersForReservationAsync(attendeeIds)).ReturnsAsync(attendees);

        var result = await CreateReservationAsync(CreateRequest(room.Id, organizer.Id, attendeeIds));

        AssertError(result.Error, "ROOM_CAPACITY_EXCEEDED");
        _reservationRepository.Verify(repository => repository.HasRoomReservationOverlapAsync(
            It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int?>()), Times.Never);
        _reservationRepository.Verify(repository => repository.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task CreateReservation_WhenOrganizerIsAnAttendee_CreatesReservationAsync()
    {
        var room = new MeetingRoom { Id = 1, Capacity = 2, RoomStatus = RoomStatus.Active };
        var organizer = new User { Id = 2 };
        var attendeeIds = new List<int> { organizer.Id };
        var attendees = new List<User> { organizer };

        _roomRepository.Setup(repository => repository.RoomExistsAsync(room.Id)).ReturnsAsync(room);
        _userRepository.Setup(repository => repository.UserExistsAsync(organizer.Id)).ReturnsAsync(organizer);
        _userRepository.Setup(repository => repository.GetUsersForReservationAsync(attendeeIds)).ReturnsAsync(attendees);
        _reservationRepository.Setup(repository => repository.HasRoomReservationOverlapAsync(
                room.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(false);
        _reservationRepository.Setup(repository => repository.AddReservationAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        var result = await CreateReservationAsync(CreateRequest(room.Id, organizer.Id, attendeeIds));

        Assert.Null(result.Error);
        Assert.NotNull(result.Reservation);
        Assert.Single(result.Reservation.Attendees);
        Assert.Same(organizer, result.Reservation.Attendees[0]);
        Assert.Equal(1, result.AttendeeCount);
        _reservationRepository.Verify(repository => repository.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task CancelReservation_WithFutureBookedReservation_CancelsAndSavesAsync()
    {
        var reservation = new Reservation
        {
            Id = 10, Status = ReservationStatus.Booked, StartTime = DateTime.UtcNow.AddHours(1)
        };
        _reservationRepository.Setup(repository => repository.GetReservationByIdAsync(reservation.Id)).ReturnsAsync(reservation);
        _reservationRepository.Setup(repository => repository.SaveReservationChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CancelReservationAsync(reservation.Id);

        Assert.Null(result.Error);
        Assert.Same(reservation, result.Reservation);
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        _reservationRepository.Verify(repository => repository.SaveReservationChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("missing", "RESERVATION_NOT_FOUND")]
    [InlineData("cancelled", "RESERVATION_NOT_BOOKED")]
    public async Task CancelReservation_WhenNotAllowed_ReturnsExpectedErrorAsync(
        string scenario, string expectedErrorCode)
    {
        var reservation = scenario == "missing" ? null : new Reservation
        {
            Id = 10,
            Status = scenario == "cancelled" ? ReservationStatus.Cancelled : ReservationStatus.Booked,
            StartTime = DateTime.UtcNow.AddHours(1)
        };
        _reservationRepository.Setup(repository => repository.GetReservationByIdAsync(10)).ReturnsAsync(reservation);

        var result = await _service.CancelReservationAsync(10);

        AssertError(result.Error, expectedErrorCode);
        _reservationRepository.Verify(repository => repository.SaveReservationChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task EndReservationEarly_ForActiveBookedReservation_CompletesAndStoresActualEndAsync()
    {
        var reservation = new Reservation
        {
            Id = 11,
            Status = ReservationStatus.Booked,
            StartTime = DateTime.UtcNow.AddMinutes(-30),
            EndTime = DateTime.UtcNow.AddMinutes(30)
        };
        _reservationRepository.Setup(repository => repository.GetReservationByIdAsync(reservation.Id)).ReturnsAsync(reservation);
        _reservationRepository.Setup(repository => repository.SaveReservationChangesAsync()).Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;
        var result = await _service.EndReservationEarlyAsync(reservation.Id);
        var after = DateTime.UtcNow;

        Assert.Null(result.Error);
        Assert.Equal(ReservationStatus.Completed, reservation.Status);
        Assert.InRange(reservation.ActualEndTime!.Value, before, after);
        _reservationRepository.Verify(repository => repository.SaveReservationChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("missing", "RESERVATION_NOT_FOUND")]
    [InlineData("not-booked", "RESERVATION_NOT_BOOKED")]
    [InlineData("not-started", "RESERVATION_NOT_STARTED")]
    [InlineData("already-ended", "RESERVATION_ALREADY_ENDED")]
    public async Task EndReservationEarly_WhenNotAllowed_ReturnsExpectedErrorAsync(
        string scenario, string expectedErrorCode)
    {
        var reservation = scenario == "missing" ? null : new Reservation
        {
            Id = 12,
            Status = scenario == "not-booked" ? ReservationStatus.Cancelled : ReservationStatus.Booked,
            StartTime = scenario == "not-started" ? DateTime.UtcNow.AddMinutes(1) : DateTime.UtcNow.AddMinutes(-30),
            EndTime = scenario == "already-ended" ? DateTime.UtcNow.AddMinutes(-1) : DateTime.UtcNow.AddMinutes(30)
        };
        _reservationRepository.Setup(repository => repository.GetReservationByIdAsync(12)).ReturnsAsync(reservation);

        var result = await _service.EndReservationEarlyAsync(12);

        AssertError(result.Error, expectedErrorCode);
        _reservationRepository.Verify(repository => repository.SaveReservationChangesAsync(), Times.Never);
    }

    private Task<CreateReservationResult> CreateReservationAsync(
        (int RoomId, int OrganizerId, string MeetingTitle, DateTime StartTime, DateTime EndTime,
            List<int> AttendeeIds) request) =>
        _service.CreateReservationAsync(
            request.RoomId,
            request.OrganizerId,
            request.MeetingTitle,
            request.StartTime,
            request.EndTime,
            request.AttendeeIds);

    private static (int RoomId, int OrganizerId, string MeetingTitle, DateTime StartTime, DateTime EndTime,
        List<int> AttendeeIds) CreateRequest(
        int roomId, int organizerId, List<int>? attendeeIds = null)
    {
        return (
            roomId,
            organizerId,
            "  Planning meeting  ",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            attendeeIds ?? []);
    }

    private static void AssertError(ReservationError? error, string expectedCode)
    {
        Assert.NotNull(error);
        Assert.Equal(expectedCode, error.ErrorCode);
    }
}
