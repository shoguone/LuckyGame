using LuckyGame.DataAccess.Context;
using LuckyGame.DataAccess.Entities;

namespace LuckyGame.DataAccess.DomainServices;

public class MatchHistoryService : IMatchHistoryService
{
    private readonly IDbContext _dbContext;

    public MatchHistoryService(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task StoreMatchHistory(MatchHistory entry)
    {
        _dbContext.MatchHistoryEntries.Add(entry);
        await _dbContext.SaveChangesAsync();
    }
}