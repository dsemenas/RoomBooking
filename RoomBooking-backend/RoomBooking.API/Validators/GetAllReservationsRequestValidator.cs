using FluentValidation;
using RoomBooking.API.Contracts.Reservations;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.API.Validators;

public class GetAllReservationsRequestValidator : AbstractValidator<GetAllReservationsRequest>
{
    public GetAllReservationsRequestValidator()
    {
        RuleFor(request => request.roomId)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ROOM_ID")
            .WithMessage("Room id must be positive number.")
            .When(request => request.roomId != null);

        RuleFor(request => request.organizerId)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ORGANIZER_ID")
            .WithMessage("Organizer id must be positive number.")
            .When(request => request.organizerId != null);

        RuleFor(request => request.status)
            .Must(status =>
                status is null ||
                (Enum.TryParse<ReservationStatus>(status, ignoreCase: true, out var parsed) &&
                 Enum.IsDefined(parsed)))
            .When(request => request.status != null)
            .WithErrorCode("INVALID_STATUS")
            .WithMessage("Status must have value of Booked, Completed or Cancelled.");
        
        RuleFor(request => request.from)
            .Must(from => from!.Value.Kind == DateTimeKind.Utc)
            .WithErrorCode("INVALID_DATE_FORMAT")
            .WithMessage("'From' must be represented in UTC format.")
            .When(request => request.from != null);

        RuleFor(request => request.to)
            .Must(to => to!.Value.Kind == DateTimeKind.Utc)
            .WithErrorCode("INVALID_DATE_FORMAT")
            .WithMessage("'To' must be represented in UTC format.")
            .When(request => request.to != null);

        RuleFor(request => request.to)
            .GreaterThanOrEqualTo(request => request.from)
            .WithErrorCode("INVALID_DATE_RANGE")
            .WithMessage("The end date must be the same as or later than the start date.")
            .When(x => x.from.HasValue && x.to.HasValue);
    }
    
}
