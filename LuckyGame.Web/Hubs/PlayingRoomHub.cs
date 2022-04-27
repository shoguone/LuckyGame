using LuckyGame.Web.Application.Interfaces;
using LuckyGame.Web.Application.Model;
using Microsoft.AspNetCore.SignalR;

namespace LuckyGame.Web.Hubs;

public class PlayingRoomHub : Hub
{
    public const string MethodWait = "wait";
    public const string MethodFull = "full";
    public const string MethodStart = "start";
    public const string MethodRound = "round";
    public const string MethodGameOver = "gameover";

    private readonly IRoomDispatcher _roomDispatcher;
    private readonly ILogger<PlayingRoomHub> _logger;

    public PlayingRoomHub(IRoomDispatcher roomDispatcher, ILogger<PlayingRoomHub> logger)
    {
        _roomDispatcher = roomDispatcher;
        _logger = logger;
    }
    
    public async Task Connect(string playerName)
    {
        _logger.LogDebug(
            "client {Context.ConnectionId} has connected. Name: {PlayerName}",
            Context.ConnectionId, playerName);
        
        var client = new Client(Context.ConnectionId, playerName);
        var roomStatus = _roomDispatcher.JoinRoom(client);
        switch (roomStatus)
        {
            case RoomStatus.Wait:
                await SendWait();
                break;
            case RoomStatus.Full:
                await SendFull();
                break;
            case RoomStatus.Start:
                await SendStart();
                await Task.Delay(TimeSpan.FromSeconds(1));
                await SendRound();
                await SendGameOver();

                _roomDispatcher.ResetRoom();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomStatus), roomStatus, $"No such {nameof(RoomStatus)}");
        }
    }
    
    private async Task SendWait()
    {
        _logger.LogDebug("sending wait to {Context.ConnectionId}", Context.ConnectionId);
        await Clients.Caller.SendAsync(MethodWait);
    }

    private async Task SendFull()
    {
        _logger.LogDebug("sending full to {Context.ConnectionId}", Context.ConnectionId);
        await Clients.Caller.SendAsync(MethodFull);
    }

    private async Task SendStart()
    {
        var clientIds = _roomDispatcher.GetClientIds();
        _logger.LogDebug("sending start to {ClientIds}", clientIds);
        await Clients.Clients(clientIds).SendAsync(MethodStart);
    }

    private async Task SendRound()
    {
        var clientIds = _roomDispatcher.GetClientIds();
        _logger.LogDebug("sending round to {ClientIds}", clientIds);
        await Clients.Clients(clientIds).SendAsync(MethodRound);
    }

    private async Task SendGameOver()
    {
        var clientIds = _roomDispatcher.GetClientIds();
        _logger.LogDebug("sending game over to {ClientIds}", clientIds);
        await Clients.Clients(clientIds).SendAsync(MethodGameOver);
    }
}
