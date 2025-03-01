﻿using CvBackend.Models;
using SqlKata.Execution;
using System.Globalization;

namespace CvBackend.Repositories;

interface IUserRepository
{
    Task InsertVisitor(UserData userData);
    Task<StatDto> GetStatictics();
    public Task<IEnumerable<UserData>> GetVisitors(bool filter, string ip, int page, int size);
}

public class UserRepository : IUserRepository
{
    private readonly QueryFactory _db;

    public UserRepository(QueryFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserData>> GetVisitors(bool filter, string ip, int page, int size)
    {
        OpenConnection();

        var users = filter
            ? await _db.Query("cv1")
                .Select("ip as Ip", "country as Country", "city as City", "os as Os", "browser as Browser", "datetime as DateTime", "provider as Org")
                .Where("ip", "<>", ip)
                .OrderByDesc("DateTime")
                .Skip(size * page)
                .Take(size)
                .GetAsync<UserData>()
            : await _db.Query("cv1")
                .Select("ip as Ip", "country as Country", "city as City", "os as Os", "browser as Browser", "datetime as DateTime", "provider as Org")
                .OrderByDesc("DateTime")
                .Skip(size * page)
                .Take(size)
                .GetAsync<UserData>();

        CloseConnection();

        return users;
    }

    public async Task<StatDto> GetStatictics()
    {
        OpenConnection();

        var query = await _db.Query("cv1")
                .Select("ip as Ip", "datetime as DateTime")
                .GetAsync<UserData>();

        var stat = query.ToArray();
        var all = stat.Length;
        var unique = stat.Select(x => x.Ip).Distinct().Count();
        var monthly = stat
            .Where(x => x.DateTime?
                .StartsWith(DateTime.UtcNow.
                    AddHours(2)
                    .ToString("yyyy-MM", CultureInfo.InvariantCulture)) ?? false)
            .Count();

        var result = new StatDto()
        {
            Unique = unique,
            All = all,
            Monthy = monthly
        };

        CloseConnection();

        return result;
    }

    public async Task InsertVisitor(UserData userData)
    {
        OpenConnection();

        await _db.Query("cv1").InsertAsync(new
        {
            ip = userData.Ip,
            country = userData.Country,
            city = userData.City,
            os = userData.Os,
            browser = userData.Browser,
            datetime = DateTime.UtcNow.AddHours(2).ToString("yyyy-MM-dd, HH:mm:ss", CultureInfo.InvariantCulture),
            provider = userData.Org
        });

        CloseConnection();
    }

    private void OpenConnection()
    {
        if (_db.Connection.State != System.Data.ConnectionState.Open)
        {
            _db.Connection.Open();
        }
    }

    private void CloseConnection()
        => _db.Connection.Close();
}
