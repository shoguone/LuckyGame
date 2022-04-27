using LuckyGame.ApplicationLogic.Interfaces;
using LuckyGame.ApplicationLogic.Model;

namespace LuckyGame.ApplicationLogic.Services;

public class RoomDispatcher : IRoomDispatcher
{
    private readonly object _roomLock = new();
    private readonly Room _room;

    public RoomDispatcher()
    {
        _room = new Room();
    }

    public IEnumerable<string> GetClientIds()
    {
        var list = new List<string>();
        if (_room.Client1?.ConnectionId != null)
        {
            list.Add(_room.Client1.ConnectionId);
        }

        if (_room.Client2?.ConnectionId != null)
        {
            list.Add(_room.Client2.ConnectionId);
        }

        return list;
    }

    public (string, string) GetClientNames()
    {
        if (_room.Client1?.Name == null)
        {
            throw new NullReferenceException(nameof(Room.Client1.Name));
        }

        if (_room.Client2?.Name == null)
        {
            throw new NullReferenceException(nameof(Room.Client2.Name));
        }

        return (_room.Client1.Name, _room.Client2.Name);
    }

    public RoomStatus JoinRoom(Client client)
    {
        lock (_roomLock)
        {
            if (_room.IsEmpty)
            {
                _room.Client1 = client;
                return RoomStatus.Wait;
            }

            if (_room.IsFull)
            {
                return RoomStatus.Full;
            }

            _room.Client2 = client;
            return RoomStatus.Start;
        }
    }

    public void ResetRoom()
    {
        _room.Reset();
    }
}
