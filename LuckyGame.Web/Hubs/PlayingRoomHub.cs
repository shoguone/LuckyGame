using Microsoft.AspNetCore.SignalR;

namespace LuckyGame.Web.Hubs;

public class PlayingRoomHub : Hub
{
    public const string MethodWait = "wait";

    private const string CurrentRoom = "TheRoom";

    public async Task Connect(string playerName)
    {
        await SendWait();
    }
    
    private async Task SendWait()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, CurrentRoom);
        await Clients.Caller.SendAsync(MethodWait);
    }

}
