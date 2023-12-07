using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.Identity;

namespace IntegrationTests.Tests;

public abstract class TestBase 
{
    protected readonly HttpClient _client;
    protected readonly IServiceScope _scope;
    protected readonly ApplicationDbContext _dbContext;
    protected readonly UserManager<ApplicationUser> _userManager;
    
    protected TestBase(VitalApiFactory vaf)
    {
        _client = vaf.Client;
        _scope = vaf.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }
}
