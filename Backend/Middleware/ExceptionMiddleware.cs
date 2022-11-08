using System.Net;
using System.Text.Json;
using Backend.Model;

namespace Backend.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (HttpException e)
        {
            await HandleExceptionAsync(context, e.StatusCode, e.Message);
        }
        catch (HttpRequestException e)
        {
            await HandleExceptionAsync(context,
                e.StatusCode == null ? (int) HttpStatusCode.InternalServerError : (int) e.StatusCode, e.Message);
        }
        catch (NotImplementedException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotImplemented, e.Message);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, e.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode status, string message)
    {
        await HandleExceptionAsync(context, (int) status, message);
    }

    private async Task HandleExceptionAsync(HttpContext context, int status, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = status,
            Message = message
        }.ToString());
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}