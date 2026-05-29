using CopilotRegisterDemo.Api.Services;
using CopilotRegisterDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<UserService>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var traceId = context.HttpContext.TraceIdentifier;
        var errors = context.ModelState.Values
            .SelectMany(item => item.Errors)
            .Select(item => string.IsNullOrWhiteSpace(item.ErrorMessage) ? "Request validation failed." : item.ErrorMessage)
            .Distinct();

        return new BadRequestObjectResult(
            Result<object?>.Failure(string.Join("; ", errors), traceId));
    };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
