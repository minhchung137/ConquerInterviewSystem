using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Workers
{
    public class SubscriptionExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionExpirationService> _logger;

        public SubscriptionExpirationService(IServiceProvider serviceProvider, ILogger<SubscriptionExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiration Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredSubscriptions();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking expired subscriptions.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CheckExpiredSubscriptions()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ConquerInterviewDbContext>();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var expiredSubs = await context.UserSubscriptions
                    .Where(s => s.Status == "Active" && s.EndDate < today)
                    .ToListAsync();

                if (expiredSubs.Any())
                {
                    _logger.LogInformation($"Found {expiredSubs.Count} expired subscriptions.");

                    foreach (var sub in expiredSubs)
                    {
                        sub.Status = "Expired";

                         var hasOtherActiveSub = context.UserSubscriptions.Any(s => s.UserId == sub.UserId && s.Status == "Active" && s.SubscriptionId != sub.SubscriptionId);
                        if (!hasOtherActiveSub)
                        {
                            var user = await context.Users.FindAsync(sub.UserId);
                            if (user != null) user.Status = false;
                        }
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Updated expired subscriptions successfully.");
                }
            }
        }
    }
}