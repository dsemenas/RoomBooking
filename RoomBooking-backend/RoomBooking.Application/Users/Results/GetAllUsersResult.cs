using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Users.Results;

public class GetAllUsersResult
{
    public string? Error { get; }
    public string? ErrorDescription { get; }
    public List<User>? Users { get; }

    public GetAllUsersResult(string? error, string? errorDescription, List<User>? users)
    {
        Error = error;
        ErrorDescription = errorDescription;
        Users = users;
    }
}