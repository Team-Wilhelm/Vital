using System.Data;
using System.Data.Common;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Vital;

namespace IntegrationTests.Setup;

public class VitalApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("vital")
            .WithUsername("root")
            .WithPassword("password")
            .Build();

    public HttpClient Client { get; private set; } = null!;
    public ApplicationDbContext DbContext { get; set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
        var testDbInitializer = scope.ServiceProvider.GetRequiredService<TestDbInitializer>();
        await testDbInitializer.Init();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
