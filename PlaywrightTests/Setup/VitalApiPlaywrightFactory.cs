using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using PlaywrightSharp;
using Respawn;
using Testcontainers.PostgreSql;
using Vital;

namespace PlaywrightTests.Setup;

public class VitalApiPlaywrightFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    public TestDbInitializer _dbInitializer = null!;
    private Process? _apiProcess;

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("vital")
            .WithUsername("root")
            .WithPassword("password")
            .Build();

    // Playwright
    private IPlaywright Playwright { get; set; }
    public IBrowser Browser { get; private set; }
    public string BaseUrl { get; } = $"http://localhost:4200";
    public HttpClient Client { get; private set; } = null!;
    public ApplicationDbContext DbContext { get; set; } = null!;

    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls("http://localhost:5261");

        builder.ConfigureTestServices(services =>
        {
            // Remove all available DbContextOptions<ApplicationDbContext> services
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            // Get connection string from your container
            var connectionString = _dbContainer.GetConnectionString();

            // Add IDbConnection to dependency injection
            services.AddScoped<IDbConnection>(container =>
            {
                var connection = new NpgsqlConnection(connectionString ?? throw new Exception("Connection string cannot be null"));
                connection.Open();
                return connection;
            });

            // Add new ApplicationDbContext service with UseNpgsql
            services.AddDbContext<ApplicationDbContext>(x =>
                x.UseNpgsql(connectionString));

            // Add TestDbInitializer
            services.AddScoped<TestDbInitializer>();
        });
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Client = CreateClient();
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        using var scope = Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await DbContext.Database.EnsureCreatedAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });

        // Add TestDbInitializer
        _dbInitializer = scope.ServiceProvider.GetRequiredService<TestDbInitializer>();
        await _dbInitializer.Init();
        

        // Start the API
        _apiProcess = new Process
        {
            StartInfo =
            {
                FileName = "dotnet",
                Arguments = "run --project ../Vital/Vital.csproj --urls http://localhost:5261",
                WorkingDirectory = "../../../",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        _apiProcess.Start();
        await _apiProcess.StandardOutput.ReadLineAsync(); // Wait till server starts

        // Playwright
        Playwright = await PlaywrightSharp.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new LaunchOptions()
        {
            Headless = false
        });
    }
    
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await Browser.CloseAsync();
        Playwright.Dispose();

        if (_apiProcess != null && !_apiProcess.HasExited)
            _apiProcess.Kill();
    }
}
