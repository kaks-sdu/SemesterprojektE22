namespace Backend.Model.Database;

public class GithubEvent
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public int ActorId { get; set; }
    public int RepoId { get; set; }
    public bool Public { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}