using LuckyGame.GameLogic.Model;

namespace LuckyGame.GameLogic.Events;

public class GameOverEventArgs : EventArgs
{
    public GameOverEventArgs(Player winner, Player loser)
    {
        Winner = winner;
        Loser = loser;
    }
    
    public Player Winner { get; }
    
    public Player Loser { get; }
}
