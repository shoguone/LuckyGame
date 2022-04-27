using LuckyGame.GameLogic.Interfaces;
using LuckyGame.GameLogic.Model;

namespace LuckyGame.GameLogic.Services;

public class GameMasterFactory : IGameMasterFactory
{
    public IGameMaster CreateGameMaster(string player1Name, string player2Name) =>
        new GameMaster(player1Name, player2Name);

    public IGameMaster CreateGameMaster(Player player1, Player player2) =>
        new GameMaster(player1, player2);
}
