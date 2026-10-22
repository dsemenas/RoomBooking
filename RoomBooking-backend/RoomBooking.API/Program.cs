using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomBooking.API.Contracts.Reservations;
using RoomBooking.API.Validators;
using RoomBooking.Application;
using RoomBooking.Application.Equipment;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Reservations;
using RoomBooking.Application.Users;
using RoomBooking.Persistence;
using RoomBooking.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<
    IValidator<GetAllReservationsRequest>,
    GetAllReservationsRequestValidator>();
builder.Services.AddScoped<
    IValidator<CreateReservationRequest>,
    CreateReservationRequestValidator>();

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("RoomBooking"));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapControllers();

app.Run();
