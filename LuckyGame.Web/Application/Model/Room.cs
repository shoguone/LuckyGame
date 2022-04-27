namespace LuckyGame.Web.Application.Model;

public class Room
{
    public Client? Client1 { get; set; }
    
    public Client? Client2 { get; set; }

    public bool IsEmpty => Client1 == null && Client2 == null;

    public bool IsFull => Client1 != null && Client2 != null;

    public Room Reset()
    {
        Client1 = null;
        Client2 = null;
        return this;
    }
}
