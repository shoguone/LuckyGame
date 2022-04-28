using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Model;
using LuckyGame.DataAccess.DomainServices;
using LuckyGame.DataAccess.Entities;
using LuckyGame.GameLogic.Events;
using LuckyGame.GameLogic.Interfaces;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IMatchHistoryService _matchHistoryService;

    public PlayingRoomHub(
        IRoomDispatcher roomDispatcher,
        ILogger<PlayingRoomHub> logger,
        IServiceProvider serviceProvider,
        IMatchHistoryService matchHistoryService)
    {
        _roomDispatcher = roomDispatcher;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _matchHistoryService = matchHistoryService;
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
                var gameMasterFactory = _serviceProvider.GetService<IGameMasterFactory>();
                if (gameMasterFactory == null)
                {
                    throw new NullReferenceException(nameof(gameMasterFactory));
                }

                var (client1Name, client2Name) = _roomDispatcher.GetClientNames();
                var gameMaster = gameMasterFactory.CreateGameMaster(client1Name, client2Name);

                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;

                gameMaster.OnRound += async (_, args) =>
                {
                    await SendRound(args);
                };
                gameMaster.OnGameOver += async (_, args) =>
                {
                    await SendGameOver(args);

                    await _matchHistoryService.StoreMatchHistory(new MatchHistory
                    {
                        WinnerName = args.Winner.Name,
                        LoserName = args.Loser.Name,
                        WinnerScore = args.Winner.Health,
                        LoserScore = args.Loser.Health,
                    });
                    
                    _roomDispatcher.ResetRoom();
                    cts.Cancel();
                };

                gameMaster.StartGame();

                // prevent Hub from disposing before the game is over
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

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
