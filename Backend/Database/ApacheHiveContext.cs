using Backend.Model;
using Backend.Model.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class ApacheHiveContext : DbContext
{
    public DbSet<GithubEvent> Tickets { get; set; } = null!;

    public ApacheHiveContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<GithubEvent>(ticket =>
        {
            ticket.HasKey(e => e.Id);
            ticket.HasIndex(e => e.Type);
            ticket.HasIndex(e => e.ActorId);
            ticket.HasIndex(e => e.RepoId);
        });
    }
}