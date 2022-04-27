using LuckyGame.ApplicationLogic.Model;

namespace LuckyGame.ApplicationLogic.Interfaces;

public interface IRoomDispatcher
{
    IEnumerable<string> GetClientIds();

    (string, string) GetClientNames();

    RoomStatus JoinRoom(Client client);
    
    void ResetRoom();
}
