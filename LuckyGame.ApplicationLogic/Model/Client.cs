namespace LuckyGame.ApplicationLogic.Model;

public class Client
{
    public Client(string connectionId, string name)
    {
        ConnectionId = connectionId;
        Name = name;
    }
    
    public string ConnectionId { get; }

    public string Name { get; }
}
