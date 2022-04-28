using LuckyGame.GameLogic.Events;

namespace LuckyGame.GameLogic.Interfaces;

public interface IGameMaster
{
    event EventHandler<RoundResultsEventArgs>? OnRound;
    event EventHandler<GameOverEventArgs>? OnGameOver;

    IGameMaster ConfigDamage(int from, int to);
    void StartGame();
}
