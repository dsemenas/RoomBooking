using RoomBooking.Domain.Helpers;

namespace RoomBooking.API.Contracts.Reservations;

public record GetAllReservationsRequest(
    int? roomId,
    int? organizerId,
    string? status,
    DateTime? from,
    DateTime? to);