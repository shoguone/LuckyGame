using LuckyGame.DataAccess.Entities;

namespace LuckyGame.DataAccess.DomainServices;

public interface IMatchHistoryService
{
    Task StoreMatchHistory(MatchHistory entry);
}