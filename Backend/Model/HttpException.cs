using System.Net;

namespace Backend.Model;

public abstract class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected HttpException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}