namespace RoomBooking.API.Contracts.Users;

using RoomBooking.API.Contracts;

public class GetAllUsersDetailsResponse
{
    public ErrorResponse? Error { get; }
    public List<UserDetailsResponse>? Users { get; }

    public GetAllUsersDetailsResponse(ErrorResponse? error, List<UserDetailsResponse>? users)
    {
        Error = error;
        Users = users;
    }
}

public class UserDetailsResponse
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public int ActiveReservationsCount { get; }

    public UserDetailsResponse(int id, string firstName, string lastName, int activeReservationsCount)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        ActiveReservationsCount = activeReservationsCount;
    }
}