using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts;
using RoomBooking.API.Contracts.Reservations;
using RoomBooking.Application.Reservations;
using RoomBooking.Application.Reservations.Results;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IValidator<GetAllReservationsRequest> _getAllReservationsRequestValidator;
    private readonly IValidator<CreateReservationRequest> _createReservationRequestValidator;

    public ReservationsController(
        IReservationService reservationService,
        IValidator<GetAllReservationsRequest> getAllReservationsRequestValidator,
        IValidator<CreateReservationRequest> createReservationRequestValidator)
    {
        _reservationService = reservationService;
        _getAllReservationsRequestValidator = getAllReservationsRequestValidator;
        _createReservationRequestValidator = createReservationRequestValidator;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllReservationsResponse>> GetAllReservationsAsync(
        [FromQuery] GetAllReservationsRequest request)
    {
        var validationResult = await _getAllReservationsRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            
            return BadRequest(new GetAllReservationsResponse(new ErrorResponse(error.ErrorCode, error.ErrorMessage), null));
        }
        
        ReservationStatus? status = request.status is null
            ? null
            : Enum.Parse<ReservationStatus>(request.status, ignoreCase: true);

        var result = await _reservationService.GetAllReservationsAsync(
            request.roomId, request.organizerId, status, request.from, request.to);

        if (result.Error != null)
        {
            var response = new GetAllReservationsResponse(
                new ErrorResponse(result.Error.ErrorCode, result.Error.ErrorMessage),
                null);

            switch (result.Error.ErrorCode)
            {
                case "ROOM_NOT_FOUND":
                case "USER_NOT_FOUND":
                    return NotFound(response);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                
            }
        }
        
        return Ok(new GetAllReservationsResponse(null, result.Reservations!.Select(reservationWithActions =>
        {
            var reservation = reservationWithActions.Reservation;
            return new ReservationDetailsResponse(
                reservation.Id,
                reservation.MeetingRoom.Name,
                $"{reservation.Organizer.FirstName} {reservation.Organizer.LastName}",
                reservation.MeetingTitle,
                reservation.StartTime,
                reservation.EndTime,
                reservationWithActions.AttendeeCount,
                reservation.Status.ToString(),
                reservationWithActions.CanBeCancelled,
                reservationWithActions.CanBeEndedEarly);
        }).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> CreateReservationAsync(
        [FromBody] CreateReservationRequest request)
    {
        var validationResult = await _createReservationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return BadRequest(new ReservationResponse(
                new ErrorResponse(error.ErrorCode, error.ErrorMessage), null));
        }

        var result = await _reservationService.CreateReservationAsync(
            request.RoomId,
            request.OrganizerId,
            request.MeetingTitle,
            request.StartTime,
            request.EndTime,
            request.AttendeeIds);
        if (result.Error != null)
        {
            var errorResponse = new ReservationResponse(
                new ErrorResponse(result.Error.ErrorCode, result.Error.ErrorMessage), null);
            switch (result.Error.ErrorCode)
            {
                case "ROOM_NOT_FOUND":
                case "USER_NOT_FOUND":
                    return NotFound(errorResponse);

                case "ROOM_BOOKING_CONFLICT":
                case "ORGANIZER_BOOKING_CONFLICT":
                case "ROOM_INACTIVE":
                    return Conflict(errorResponse);

                case "INVALID_ROOM_ID":
                case "INVALID_ORGANIZER_ID":
                case "INVALID_MEETING_TITLE":
                case "INVALID_ATTENDEES_COUNT":
                case "INVALID_ATTENDEES":
                case "DUPLICATE_ATTENDEES":
                case "INVALID_DATE_FORMAT":
                case "INVALID_TIME_RANGE":
                case "START_TIME_NOT_IN_FUTURE":
                case "MAX_DURATION_EXCEEDED":
                case "BOOKING_HORIZON_EXCEEDED":
                case "ROOM_CAPACITY_EXCEEDED":
                    return BadRequest(errorResponse);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        var reservation = result.Reservation!;
        
        var response = new ReservationResponse(null, new ReservationDetailsResponse(
            reservation.Id,
            reservation.MeetingRoom.Name,
            $"{reservation.Organizer.FirstName} {reservation.Organizer.LastName}",
            reservation.MeetingTitle,
            reservation.StartTime,
            reservation.EndTime,
            result.AttendeeCount,
            reservation.Status.ToString()));
        return StatusCode(StatusCodes.Status201Created, response);
    }


    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ReservationResponse>> CancelReservationAsync(int id)
    {
        if (id <= 0)
        {
            var errorResponse = new ReservationResponse(
                new ErrorResponse("INVALID_RESERVATION_ID", "Reservation id must be a positive integer."), null);
            return BadRequest(errorResponse);
        }

        var result = await _reservationService.CancelReservationAsync(id);
        if (result.Error != null)
        {
            var errorResponse = new ReservationResponse(
                new ErrorResponse(result.Error.ErrorCode, result.Error.ErrorMessage), null);
            switch (result.Error.ErrorCode)
            {
                case "RESERVATION_NOT_FOUND":
                    return NotFound(errorResponse);
                
                case "RESERVATION_NOT_BOOKED":
                    return Conflict(errorResponse);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        var reservation = result.Reservation!;
        return Ok(new ReservationResponse(null, new ReservationDetailsResponse(
            reservation.Id,
            reservation.MeetingRoom.Name,
            $"{reservation.Organizer.FirstName} {reservation.Organizer.LastName}",
            reservation.MeetingTitle,
            reservation.StartTime,
            reservation.EndTime,
            result.AttendeeCount,
            reservation.Status.ToString())));
    }

    [HttpPost("{id:int}/end-early")]
    public async Task<ActionResult<ReservationResponse>> EndReservationEarlyAsync(int id)
    {
        if (id <= 0)
        {
            var errorResponse = new ReservationResponse(
                new ErrorResponse("INVALID_RESERVATION_ID", "Reservation id must be a positive integer."), null);
            return BadRequest(errorResponse);
        }
        
        var result = await _reservationService.EndReservationEarlyAsync(id);
        if (result.Error is not null)
        {
            var errorResponse = new ReservationResponse(
                new ErrorResponse(result.Error.ErrorCode, result.Error.ErrorMessage), null);
            switch (result.Error.ErrorCode)
            {
                case "RESERVATION_NOT_FOUND":
                    return NotFound(errorResponse);

                case "RESERVATION_NOT_BOOKED":
                case "RESERVATION_NOT_STARTED":
                case "RESERVATION_ALREADY_ENDED":
                    return Conflict(errorResponse);

                default:
                    return BadRequest(errorResponse);
            }
        }

        var reservation = result.Reservation!;
        return Ok(new ReservationResponse(null, new ReservationDetailsResponse(
            reservation.Id,
            reservation.MeetingRoom.Name,
            $"{reservation.Organizer.FirstName} {reservation.Organizer.LastName}",
            reservation.MeetingTitle,
            reservation.StartTime,
            reservation.EndTime,
            result.AttendeeCount,
            reservation.Status.ToString())));
    }
}
