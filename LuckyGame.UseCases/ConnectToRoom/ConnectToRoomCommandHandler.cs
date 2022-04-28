using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Model;
using LuckyGame.DataAccess.DomainServices;
using LuckyGame.DataAccess.Entities;
using LuckyGame.GameLogic.Interfaces;
using MediatR;

namespace LuckyGame.UseCases.ConnectToRoom;

public class ConnectToRoomCommandHandler : IRequestHandler<ConnectToRoomCommand, Unit>
{
    private readonly IRoomDispatcher _roomDispatcher;
    private readonly IMatchHistoryService _matchHistoryService;
    private readonly IGameMasterFactory _gameMasterFactory;

    public ConnectToRoomCommandHandler(
        IRoomDispatcher roomDispatcher,
        IMatchHistoryService matchHistoryService,
        IGameMasterFactory gameMasterFactory)
    {
        _roomDispatcher = roomDispatcher;
        _matchHistoryService = matchHistoryService;
        _gameMasterFactory = gameMasterFactory;
    }

    public async Task<Unit> Handle(ConnectToRoomCommand command, CancellationToken cancellationToken)
    {
        var client = command.Client;
        var roomStatus = _roomDispatcher.JoinRoom(client);
        switch (roomStatus)
        {
            case RoomStatus.Wait:
                await command.SendWait();
                break;
            case RoomStatus.Full:
                await command.SendFull();
                break;
            case RoomStatus.Start:
                await command.SendStart();

                var (client1Name, client2Name) = _roomDispatcher.GetClientNames();
                var gameMaster = _gameMasterFactory.CreateGameMaster(client1Name, client2Name);

                var keepControlFlowCts = new CancellationTokenSource();
                var keepControlFlowCToken = keepControlFlowCts.Token;

                gameMaster.OnRound += async (_, args) => { await command.SendRound(args); };
                gameMaster.OnGameOver += async (_, args) =>
                {
                    await command.SendGameOver(args);

                    await _matchHistoryService.StoreMatchHistory(new MatchHistory
                    {
                        WinnerName = args.Winner.Name,
                        LoserName = args.Loser.Name,
                        WinnerScore = args.Winner.Health,
                        LoserScore = args.Loser.Health,
                    });

                    _roomDispatcher.ResetRoom();
                    keepControlFlowCts.Cancel();
                };

                gameMaster.StartGame();

                // prevent caller Hub from disposing before the game is over
                while (!keepControlFlowCToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomStatus), roomStatus, $"No such {nameof(RoomStatus)}");
        }

        return Unit.Value;
    }
}
