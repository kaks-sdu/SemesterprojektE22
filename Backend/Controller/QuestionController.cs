using Backend.Model.Request;
using Backend.Model.Response;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[Route("api/questions")]
public class QuestionController
{
    private readonly QuestionService _questionService;

    public QuestionController(QuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<AnswerDto> AskQuestion([FromBody] QuestionDto question)
    {
       var answer = await _questionService.AskQuestion("You you doing?");
        return new AnswerDto { Answer = "asd" };
    }
}