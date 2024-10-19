using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Team_12.DBContext;
using Team_12.Models;

public class EventExpirationService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventExpirationService> _logger;

    public EventExpirationService(IServiceScopeFactory scopeFactory, ILogger<EventExpirationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Expiration Service is starting.");

        // Delay the first execution by 1 minute to allow other services to initialize
        _timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1),
            TimeSpan.FromHours(1)); // Run every hour after the initial delay

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        try
        {
            _logger.LogInformation("Checking for expired events.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Team12DbContext>();

                var expiredEvents = dbContext.Events
                    .Where(e => e.EndDate < DateTime.UtcNow && e.IsActive)
                    .ToList();

                foreach (var expiredEvent in expiredEvents)
                {
                    expiredEvent.IsActive = false;
                    _logger.LogInformation($"Marked event {expiredEvent.EventId} as inactive (expired).");
                }

                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking for expired events.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Expiration Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}