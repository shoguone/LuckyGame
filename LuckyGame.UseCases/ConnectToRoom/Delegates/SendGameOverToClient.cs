using LuckyGame.GameLogic.Events;

namespace LuckyGame.UseCases.ConnectToRoom.Delegates;

public delegate Task SendGameOverToClient(GameOverEventArgs args);
