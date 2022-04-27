using LuckyGame.Web.Application.Model;

namespace LuckyGame.Web.Application.Interfaces;

public interface IRoomDispatcher
{
    IEnumerable<string> GetClientIds();

    RoomStatus JoinRoom(Client client);
    
    void ResetRoom();
}
