using Microsoft.AspNetCore.Identity;
using Models.Identity;

namespace Vital.Core.Services;

public class UserCleanupService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer? _timer;

    public UserCleanupService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


    private List<ApplicationUser> GetUnverifiedUsers()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return userManager.Users.Where(x => !x.EmailConfirmed).ToList();
    }


    private async Task DeleteUserAsync(Guid userId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByIdAsync(userId.ToString());
        await userManager.DeleteAsync(user!);
    }

    private void DoWork(object? state)
    {
        
        var unverifiedUsers = GetUnverifiedUsers();
        unverifiedUsers.ForEach(u =>
        {
            if (u.VerifyEmailTokenExpirationDate < DateTime.UtcNow)
            {
                DeleteUserAsync(u.Id);
            }
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
