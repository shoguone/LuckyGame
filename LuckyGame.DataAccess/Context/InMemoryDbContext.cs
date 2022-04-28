using LuckyGame.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyGame.DataAccess.Context;

public class InMemoryDbContext : DbContext, IDbContext
{
    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<MatchHistory> MatchHistoryEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MatchHistory>()
            .HasKey(x => x.Id);
    }
}