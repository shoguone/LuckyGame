using LuckyGame.GameLogic.Events;

namespace LuckyGame.UseCases.ConnectToRoom.Delegates;

public delegate Task SendRoundToClient(RoundResultsEventArgs args);
