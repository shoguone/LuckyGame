namespace LuckyGame.DataAccess.Entities;

public class MatchHistory
{
    public Guid Id { get; set; }

    public string WinnerName { get; set; }
    
    public string LoserName { get; set; }
    
    public int WinnerScore { get; set; }
    
    public int LoserScore { get; set; }
    
}