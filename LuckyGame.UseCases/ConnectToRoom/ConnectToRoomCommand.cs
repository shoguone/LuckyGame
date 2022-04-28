using LuckyGame.ApplicationLogic.Model;
using LuckyGame.UseCases.ConnectToRoom.Delegates;
using MediatR;

namespace LuckyGame.UseCases.ConnectToRoom;

public class ConnectToRoomCommand : IRequest<Unit>
{
    public ConnectToRoomCommand(
        Client client, 
        SendToClient sendWait, 
        SendToClient sendFull, 
        SendToClient sendStart, 
        SendRoundToClient sendRound, 
        SendGameOverToClient sendGameOver)
    {
        Client = client;
        SendWait = sendWait;
        SendFull = sendFull;
        SendStart = sendStart;
        SendRound = sendRound;
        SendGameOver = sendGameOver;
    }
    
    public Client Client { get; }

    public SendToClient SendWait { get; }
    
    public SendToClient SendFull { get; }
    
    public SendToClient SendStart { get; }
    
    public SendRoundToClient SendRound { get; }
    
    public SendGameOverToClient SendGameOver { get; }
}
