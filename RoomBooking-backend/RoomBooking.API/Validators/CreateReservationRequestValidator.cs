using FluentValidation;
using RoomBooking.API.Contracts.Reservations;

namespace RoomBooking.API.Validators;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator(IConfiguration configuration)
    {
        if (!int.TryParse(configuration["BookingRules:MaxDurationDays"], out var maxDurationDays) ||
            maxDurationDays <= 0)
            throw new InvalidOperationException("BookingRules:MaxDurationDays must be a positive integer.");
        if (!int.TryParse(configuration["BookingRules:MaxBookingHorizonDays"], out var maxBookingHorizonDays) ||
            maxBookingHorizonDays <= 0)
            throw new InvalidOperationException("BookingRules:MaxBookingHorizonDays must be a positive integer.");

        RuleFor(request => request.RoomId)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ROOM_ID")
            .WithMessage("RoomId must be a positive identifier.");

        RuleFor(request => request.OrganizerId)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ORGANIZER_ID")
            .WithMessage("OrganizerId must be a positive identifier.");

        RuleFor(request => request.MeetingTitle)
            .Must(title => !string.IsNullOrWhiteSpace(title) && title.Trim().Length <= 50)
            .WithErrorCode("INVALID_MEETING_TITLE")
            .WithMessage("Meeting title is required and must not exceed 50 characters.");

        RuleFor(request => request.AttendeeIds)
            .NotEmpty()
            .WithErrorCode("INVALID_ATTENDEES_COUNT")
            .WithMessage("At least one attendee must be selected.");

        RuleForEach(request => request.AttendeeIds)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ATTENDEES")
            .WithMessage("AttendeeIds must contain only positive user identifiers.");

        RuleFor(request => request.AttendeeIds)
            .Must(attendeeIds => attendeeIds is null || attendeeIds.Count == attendeeIds.Distinct().Count())
            .WithErrorCode("DUPLICATE_ATTENDEES")
            .WithMessage("AttendeeIds must not contain duplicate user identifiers.");

        RuleFor(request => request.StartTime)
            .Must(startTime => startTime.Kind == DateTimeKind.Utc)
            .WithErrorCode("INVALID_DATE_FORMAT")
            .WithMessage("StartTime must be represented in UTC format.");

        RuleFor(request => request.EndTime)
            .Must(endTime => endTime.Kind == DateTimeKind.Utc)
            .WithErrorCode("INVALID_DATE_FORMAT")
            .WithMessage("EndTime must be represented in UTC format.");

        RuleFor(request => request.EndTime)
            .GreaterThan(request => request.StartTime)
            .WithErrorCode("INVALID_TIME_RANGE")
            .WithMessage("EndTime must be later than StartTime.");

        RuleFor(request => request.StartTime)
            .Must(startTime => startTime > DateTime.UtcNow)
            .WithErrorCode("START_TIME_NOT_IN_FUTURE")
            .WithMessage("Reservation start time must be in the future.");

        RuleFor(request => request.EndTime)
            .Must((request, endTime) => endTime - request.StartTime <= TimeSpan.FromDays(maxDurationDays))
            .WithErrorCode("MAX_DURATION_EXCEEDED")
            .WithMessage($"A reservation cannot last longer than {maxDurationDays} days.")
            .When(request => request.EndTime > request.StartTime);

        RuleFor(request => request.StartTime)
            .Must(startTime => startTime <= DateTime.UtcNow.AddDays(maxBookingHorizonDays))
            .WithErrorCode("BOOKING_HORIZON_EXCEEDED")
            .WithMessage($"A reservation cannot start more than {maxBookingHorizonDays} days in the future.");
    }
}
