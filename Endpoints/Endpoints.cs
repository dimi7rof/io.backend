using CvBackend.Models;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System.Globalization;

namespace CvBackend.Endpoints;

internal static class Endpoints
{
    internal static void RegisterEndpoints(this WebApplication app)
    {
        app.MapPost("api/user", SaveUser);
        app.MapGet("api/logs", GetLogs);
    }

    internal static async Task<IResult> SaveUser(QueryFactory db, UserData userData)
    {
        if (db.Connection.State != System.Data.ConnectionState.Open)
        {
            db.Connection.Open();
        }

        await db.Query("cv1").InsertAsync(new
        {
            ip = userData.Ip,
            country = userData.Country,
            city = userData.City,
            os = userData.Os,
            browser = userData.Browser,
            datetime = DateTime.UtcNow.AddHours(2).ToString("yyyy-MM-dd, HH:mm:ss", CultureInfo.InvariantCulture),
            provider = userData.Org
        });

        db.Connection.Close();

        return Results.Ok();
    }

    internal static async Task<IResult> GetLogs(QueryFactory db, [FromQuery] bool filter, [FromQuery] string ip, [FromQuery] int page, [FromQuery] int size)
    {
        if (db.Connection.State != System.Data.ConnectionState.Open)
        {
            db.Connection.Open();
        }

        var users = filter
            ? await db.Query("cv1")
                .Select("ip as Ip", "country as Country", "city as City", "os as Os", "browser as Browser", "datetime as DateTime", "provider as Org")
                .Where("ip", "<>", ip)
                .OrderByDesc("DateTime")
                .Skip(size * page)
                .Take(size)
                .GetAsync<UserData>()
            : await db.Query("cv1")
                .Select("ip as Ip", "country as Country", "city as City", "os as Os", "browser as Browser", "datetime as DateTime", "provider as Org")
                .OrderByDesc("DateTime")
                .Skip(size * page)
                .Take(size)
                .GetAsync<UserData>();

        db.Connection.Close();

        return Results.Ok(users);
    }
}
