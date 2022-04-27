using LuckyGame.GameLogic.Model;

namespace LuckyGame.GameLogic.Events;

public class RoundResultsEventArgs : EventArgs
{
    public RoundResultsEventArgs(int player1HealthChange, int player2HealthChange, Player player1, Player player2)
    {
        Player1HealthChange = player1HealthChange;
        Player2HealthChange = player2HealthChange;
        Player1 = player1;
        Player2 = player2;
    }

    public int Player1HealthChange { get; }

    public int Player2HealthChange { get; }

    public Player Player1 { get; }

    public Player Player2 { get; }
}
