using LuckyGame.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Domain
// not yet

// Infrastructure
// not yet

// Application
// not yet

// Frameworks
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<PlayingRoomHub>("/play");

app.Run();
