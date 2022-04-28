using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Services;
using LuckyGame.DataAccess.Context;
using LuckyGame.DataAccess.DomainServices;
using LuckyGame.GameLogic.Interfaces;
using LuckyGame.GameLogic.Services;
using LuckyGame.UseCases.ConnectToRoom;
using LuckyGame.Web.Hubs;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Domain
builder.Services.AddScoped<IGameMasterFactory, GameMasterFactory>();
builder.Services.AddScoped<IMatchHistoryService, MatchHistoryService>();

// Infrastructure
builder.Services.AddDbContext<IDbContext, InMemoryDbContext>(options =>
    options.UseInMemoryDatabase("in-memory-db"));

// Application
builder.Services.AddSingleton<IRoomDispatcher, RoomDispatcher>();

// Frameworks
builder.Services.AddSignalR();
builder.Services.AddMediatR(typeof(ConnectToRoomCommandHandler));

var app = builder.Build();

app.MapHub<PlayingRoomHub>("/play");

app.Run();
