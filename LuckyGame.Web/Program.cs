using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Services;
using LuckyGame.GameLogic.Interfaces;
using LuckyGame.GameLogic.Services;
using LuckyGame.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Domain
builder.Services.AddScoped<IGameMasterFactory, GameMasterFactory>();

// Infrastructure
// not yet

// Application
builder.Services.AddSingleton<IRoomDispatcher, RoomDispatcher>();

// Frameworks
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<PlayingRoomHub>("/play");

app.Run();
