using System.Text.Json.Serialization;
using Backend.Database;
using Backend.Middleware;
using Backend.Model;
using Backend.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var env = builder.Environment;

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

services.AddDbContext<ApacheHiveContext>(options =>
{
    options.UseApacheHive(builder.Configuration.GetConnectionString("ApacheHiveConnection")!);
});

services.AddScoped(typeof(ICache<,>), typeof(Cache<,>));
services.AddScoped<QuestionService>();

services.AddSingleton<ExceptionMiddleware>();
services.AddSingleton<MemoryCache>();

services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        var converters = options.JsonSerializerOptions.Converters;
        converters.Add(new JsonStringEnumConverter());
    });

// https://localhost:7132/swagger/index.html
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

using var app = builder.Build();

app.UseCors(policy =>
{
    policy
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader();
});
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();