using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain.Entities;

namespace RoomBooking.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();

    public DbSet<Equipment> Equipment => Set<Equipment>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);

            user.Property(u => u.FirstName).IsRequired().HasMaxLength(50);

            user.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Equipment>(equipment =>
        {
            equipment.HasKey(eq => eq.Id);

            equipment.Property(eq => eq.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<MeetingRoom>(meetingRoom =>
        {
            meetingRoom.HasKey(mr => mr.Id);

            meetingRoom.Property(mr => mr.Name).IsRequired().HasMaxLength(50);

            meetingRoom.Property(mr => mr.Capacity).IsRequired();

            meetingRoom.Property(mr => mr.Floor).IsRequired();

            meetingRoom.Property(mr => mr.RoomStatus).IsRequired();

            meetingRoom.HasMany(mr => mr.Equipment)
                .WithMany(eq => eq.MeetingRooms);
        });

        modelBuilder.Entity<Reservation>(reservation =>
        {
            reservation.HasKey(r => r.Id);

            reservation.HasOne(r => r.MeetingRoom)
                .WithMany(mr => mr.Reservations)
                .HasForeignKey(r => r.RoomId).IsRequired();

            reservation.HasOne(r => r.Organizer)
                .WithMany(o => o.OrganizedReservations)
                .HasForeignKey(r => r.OrganizerId).IsRequired();

            reservation.HasMany(r => r.Attendees)
                .WithMany(u => u.AttendingReservations);

            reservation.Property(r => r.MeetingTitle).IsRequired().HasMaxLength(50);

            reservation.Property(r => r.StartTime).IsRequired();

            reservation.Property(r => r.EndTime).IsRequired();

            reservation.Property(r => r.Status).IsRequired();
        });
    }
}
