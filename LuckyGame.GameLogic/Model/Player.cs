namespace LuckyGame.GameLogic.Model;

public class Player
{
    private const int DefaultHealth = 10;

    public Player(string name, int health = DefaultHealth)
    {
        Name = name;
        Health = health;
    }
    
    public string Name { get; }

    public int Health { get; private set; }

    public void ApplyDamage(int damage)
    {
        Health -= damage;
    }
}
