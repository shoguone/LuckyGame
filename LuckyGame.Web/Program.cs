using LuckyGame.Web.Application.Interfaces;
using LuckyGame.Web.Application.Service;
using LuckyGame.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Domain
// not yet

// Infrastructure
// not yet

// Application
builder.Services.AddSingleton<IRoomDispatcher, RoomDispatcher>();

// Frameworks
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<PlayingRoomHub>("/play");

app.Run();
