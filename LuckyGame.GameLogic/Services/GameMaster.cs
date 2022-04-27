using LuckyGame.GameLogic.Events;
using LuckyGame.GameLogic.Interfaces;
using LuckyGame.GameLogic.Model;

namespace LuckyGame.GameLogic.Services;

public class GameMaster : IGameMaster
{
    private const int DefaultDamageFrom = 0;
    private const int DefaultDamageTo = 2;

    private readonly TimeSpan _roundLength = TimeSpan.FromSeconds(1);

    private readonly Player _player1;
    private readonly Player _player2;
    private readonly RoundDispatcher _roundDispatcher;

    private int _randomDamageFrom = DefaultDamageFrom;
    private int _randomDamageTo = DefaultDamageTo;

    public GameMaster(string player1Name, string player2Name)
        : this(new Player(player1Name), new Player(player2Name))
    {
    }

    public GameMaster(Player player1, Player player2)
    {
        _player1 = player1;
        _player2 = player2;
        _roundDispatcher = new RoundDispatcher(_roundLength);
        _roundDispatcher.OnRound += (_, _) => PlayRound();
    }

    public event EventHandler<RoundResultsEventArgs>? OnRound;
    public event EventHandler<GameOverEventArgs>? OnGameOver;

    public IGameMaster ConfigDamage(int from, int to)
    {
        _randomDamageFrom = from;
        _randomDamageTo = to;
        return this;
    }
    
    public void StartGame()
    {
        _roundDispatcher.Start();
    }

    private void PlayRound()
    {
        var damage1 = GetRandomDamage();
        var damage2 = GetRandomDamagePreventingDraw();
        _player1.ApplyDamage(damage1);
        _player2.ApplyDamage(damage2);

        OnRound?.Invoke(this, new RoundResultsEventArgs(
            -damage1, -damage2, _player1, _player2));

        if (_player1.Health <= 0)
        {
            OnGameOver?.Invoke(this, new GameOverEventArgs(_player2, _player1));
            EndGame();
        }
        else if (_player2.Health <= 0)
        {
            OnGameOver?.Invoke(this, new GameOverEventArgs(_player1, _player2));
            EndGame();
        }

        int GetRandomDamagePreventingDraw()
        {
            return _player1.Health <= 0
                ? GetRandomDamage(@from: 0, to: _player2.Health - 1)
                : GetRandomDamage();
        }
    }

    private int GetRandomDamage(int? from = null, int? to = null) =>
        Random.Shared.Next(
            from ?? _randomDamageFrom,
            (to ?? _randomDamageTo) + 1);

    private void EndGame()
    {
        _roundDispatcher.Stop();
    }
}
