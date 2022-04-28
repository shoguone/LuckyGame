using LuckyGame.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyGame.DataAccess.Context;

public interface IDbContext
{
    DbSet<MatchHistory> MatchHistoryEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken token = default);
    
}