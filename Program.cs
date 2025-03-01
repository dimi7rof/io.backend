using CvBackend.Endpoints;
using CvBackend.Repositories;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(["https://dimi7rof.github.io"])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

builder.Services.AddScoped(provider
    => new QueryFactory(
            new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")),
            new PostgresCompiler()));

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.UseCors();

app.RegisterEndpoints();

app.Run();
