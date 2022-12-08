using Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public class QuestionService
{
    private readonly ApacheHiveContext _database;

    public QuestionService(ApacheHiveContext database)
    {
        _database = database;
    }

    public async Task<string> AskQuestion(string question)
    {
        var githubEvents = await _database.Tickets
            .FromSqlRaw("SELECT * from push_events")
            .ToListAsync();
        return await Task.FromResult("42");
    }
}