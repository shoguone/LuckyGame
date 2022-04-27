using LuckyGame.GameLogic.Model;

namespace LuckyGame.GameLogic.Interfaces;

public interface IGameMasterFactory
{
    IGameMaster CreateGameMaster(string player1Name, string player2Name);

    IGameMaster CreateGameMaster(Player player1, Player player2);
}
