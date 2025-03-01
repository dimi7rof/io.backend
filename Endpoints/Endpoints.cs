using CvBackend.Models;
using CvBackend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CvBackend.Endpoints;

internal static class Endpoints
{
    internal static void RegisterEndpoints(this WebApplication app)
    {
        app.MapPost("api/insertVisitor", InsertVisitor);
        app.MapGet("api/getVisitors", GetVisitors);
        app.MapGet("api/getStatistics", GetStatistics);
    }

    internal static async Task<IResult> InsertVisitor([FromServices] IUserRepository repo, [FromBody] UserData userData)
    {
        await repo.InsertVisitor(userData);

        return Results.Ok();
    }

    internal static async Task<IResult> GetVisitors(
        [FromServices] IUserRepository repo,
        [FromQuery] bool filter,
        [FromQuery] string ip,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        var users = await repo.GetVisitors(filter, ip, page, size);

        return Results.Ok(users);
    }

    internal static async Task<IResult> GetStatistics([FromServices] IUserRepository repo)
    {
        var stat = await repo.GetStatictics();

        return Results.Ok(stat);
    }
}
