using CvBackend.Endpoints;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(["http://localhost:4200", "https://dimi7rof.github.io"])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

builder.Services.AddSingleton(provider
    => new QueryFactory(
            new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")),
            new PostgresCompiler()));

var app = builder.Build();

app.UseCors();

app.RegisterEndpoints();

app.Run();
