using Backend.Database;

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
        return await Task.FromResult("42");
    }
}