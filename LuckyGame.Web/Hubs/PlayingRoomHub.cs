using MediatR;
using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Model;
using LuckyGame.GameLogic.Events;
using LuckyGame.UseCases.ConnectToRoom;
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
    private readonly ISender _sender;

    public PlayingRoomHub(
        IRoomDispatcher roomDispatcher,
        ILogger<PlayingRoomHub> logger,
        ISender sender)
    {
        _roomDispatcher = roomDispatcher;
        _logger = logger;
        _sender = sender;
    }
    
    public async Task Connect(string playerName)
    {
        _logger.LogDebug(
            "client {Context.ConnectionId} has connected. Name: {PlayerName}",
            Context.ConnectionId, playerName);
        
        var client = new Client(Context.ConnectionId, playerName);
        var _ = await _sender.Send(new ConnectToRoomCommand(
            client,
            SendWait,
            SendFull,
            SendStart,
            SendRound,
            SendGameOver));
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

    private async Task SendRound(RoundResultsEventArgs args)
    {
        var clientIds = _roomDispatcher.GetClientIds();
        _logger.LogDebug("sending round to {ClientIds}", clientIds);
        await Clients.Clients(clientIds).SendAsync(MethodRound, args);
    }

    private async Task SendGameOver(GameOverEventArgs args)
    {
        var clientIds = _roomDispatcher.GetClientIds();
        _logger.LogDebug("sending game over to {ClientIds}", clientIds);
        await Clients.Clients(clientIds).SendAsync(MethodGameOver, args);
    }
}
