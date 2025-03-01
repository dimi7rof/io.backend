using CvBackend.Models;
using CvBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CvBackend.Endpoints;

internal static class Endpoints
{
    internal static void RegisterEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("api");

        api.MapPost("insertVisitor", InsertVisitor);
        api.MapGet("getVisitors", GetVisitors);
        api.MapGet("getStatistics", GetStatistics);
    }

    internal static async Task<IResult> InsertVisitor(
        [FromServices] IUserRepository repo,
        [FromBody] UserData userData,
        [FromServices] ILogger<Program> logger)
    {
        await repo.InsertVisitor(userData);

        logger.LogInformation("Inserted visitor: {Visitors}", userData.Ip);

        return Results.Ok();
    }

    internal static async Task<IResult> GetVisitors(
        [FromServices] IUserRepository repo,
        [FromQuery] bool filter,
        [FromQuery] string ip,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromServices] ILogger<Program> logger)
    {
        var users = await repo.GetVisitors(filter, ip, page, size);

        logger.LogInformation("Found {Visitors} visitors", users.Count());

        return Results.Ok(users);
    }

    internal static async Task<IResult> GetStatistics(
        [FromServices] IUserRepository repo,
        [FromServices] ILogger<Program> logger)
    {
        var stat = await repo.GetStatictics();

        logger.LogInformation("Statistics: {Stat}", JsonSerializer.Serialize(stat));

        return Results.Ok(stat);
    }
}
