using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Helpers;

namespace RoomBooking.Persistence;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        var equipment = db.Equipment.ToList();
        var equipmentNames = new[] { "Projector", "TV screen", "Video conferencing", "Whiteboard" };
        foreach (var equipmentName in equipmentNames)
        {
            if (equipment.Any(item => item.Name == equipmentName))
                continue;

            var newEquipment = new Equipment
            {
                Name = equipmentName,
                MeetingRooms = []
            };
            db.Equipment.Add(newEquipment);
            equipment.Add(newEquipment);
        }

        if (!db.MeetingRooms.Any())
        {
            var projector = equipment.Single(item => item.Name == "Projector");
            var tv = equipment.Single(item => item.Name == "TV screen");
            var video = equipment.Single(item => item.Name == "Video conferencing");
            var whiteboard = equipment.Single(item => item.Name == "Whiteboard");

            db.MeetingRooms.AddRange(
                new MeetingRoom
                {
                    Name = "Aurora", Capacity = 12, Floor = 1, RoomStatus = RoomStatus.Active,
                    Equipment = [projector, video, whiteboard], Reservations = []
                },
                new MeetingRoom
                {
                    Name = "Baltic", Capacity = 6, Floor = 1, RoomStatus = RoomStatus.Active,
                    Equipment = [tv, whiteboard], Reservations = []
                },
                new MeetingRoom
                {
                    Name = "Cedar", Capacity = 20, Floor = 2, RoomStatus = RoomStatus.Active,
                    Equipment = [projector, tv, video, whiteboard], Reservations = []
                },
                new MeetingRoom
                {
                    Name = "Dune", Capacity = 4, Floor = 2, RoomStatus = RoomStatus.Active,
                    Equipment = [tv], Reservations = []
                },
                new MeetingRoom
                {
                    Name = "Ember", Capacity = 8, Floor = 3, RoomStatus = RoomStatus.Inactive,
                    Equipment = [video], Reservations = []
                },
                new MeetingRoom
                {
                    Name = "Fjord", Capacity = 10, Floor = 3, RoomStatus = RoomStatus.Active,
                    Equipment = [projector, whiteboard], Reservations = []
                });
        }

        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new User
                {
                    FirstName = "Alex", LastName = "Morgan", OrganizedReservations = [], AttendingReservations = []
                },
                new User
                {
                    FirstName = "Jordan", LastName = "Lee", OrganizedReservations = [], AttendingReservations = []
                },
                new User
                {
                    FirstName = "Taylor", LastName = "Kim", OrganizedReservations = [], AttendingReservations = []
                },
                new User
                {
                    FirstName = "Casey", LastName = "Patel", OrganizedReservations = [], AttendingReservations = []
                },
                new User
                {
                    FirstName = "Riley", LastName = "Chen", OrganizedReservations = [], AttendingReservations = []
                });
        }

        db.SaveChanges();

        if (!db.Reservations.Any())
        {
            var room = db.MeetingRooms.OrderBy(item => item.Id).FirstOrDefault();
            var users = db.Users.OrderBy(item => item.Id).ToList();

            if (room != null && users.Count > 0)
            {
                var attendees = users.Take(Math.Min(room.Capacity, users.Count)).ToList();
                var now = DateTime.UtcNow;
                var completedStart = now.Date.AddDays(-2).AddHours(9);
                var bookedStart = now.Date.AddDays(2).AddHours(9);
                var cancelledStart = now.Date.AddDays(4).AddHours(9);

                db.Reservations.AddRange(
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Quarterly planning",
                        StartTime = completedStart,
                        EndTime = completedStart.AddHours(1),
                        ActualEndTime = completedStart.AddHours(1),
                        Status = ReservationStatus.Completed
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Product roadmap review",
                        StartTime = bookedStart,
                        EndTime = bookedStart.AddHours(1),
                        Status = ReservationStatus.Booked
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Design workshop",
                        StartTime = cancelledStart,
                        EndTime = cancelledStart.AddHours(2),
                        Status = ReservationStatus.Cancelled
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Budget review",
                        StartTime = now.Date.AddDays(-3).AddHours(10),
                        EndTime = now.Date.AddDays(-3).AddHours(11),
                        Status = ReservationStatus.Cancelled
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Team retrospective",
                        StartTime = now.Date.AddDays(-4).AddHours(10),
                        EndTime = now.Date.AddDays(-4).AddHours(11),
                        Status = ReservationStatus.Cancelled
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Security planning",
                        StartTime = now.Date.AddDays(-5).AddHours(10),
                        EndTime = now.Date.AddDays(-5).AddHours(11),
                        Status = ReservationStatus.Cancelled
                    },
                    new Reservation
                    {
                        RoomId = room.Id,
                        MeetingRoom = room,
                        OrganizerId = users[0].Id,
                        Organizer = users[0],
                        Attendees = attendees,
                        MeetingTitle = "Operations sync",
                        StartTime = now.Date.AddDays(-6).AddHours(10),
                        EndTime = now.Date.AddDays(-6).AddHours(11),
                        Status = ReservationStatus.Cancelled
                    });
            }
        }

        db.SaveChanges();
    }
}
