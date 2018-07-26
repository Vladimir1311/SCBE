using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SituationCenterCore.Models.TokenAuthModels;
using Microsoft.Extensions.DependencyInjection;
using SituationCenterCore.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SituationCenterCore.Services.HostedServices
{
    public class RefreshTokenRemover : IHostedService
    {
        private Timer timer;
        private readonly ILogger<RefreshTokenRemover> logger;
        private readonly IOptions<JwtOptions> options;
        private readonly IServiceProvider serviceProvider;

        public RefreshTokenRemover(
            ILogger<RefreshTokenRemover> logger,
            IOptions<JwtOptions> options,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.options = options;
            this.serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(TryWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            logger.LogInformation("Started service");
            return Task.CompletedTask;
        }

        private void TryWork(object state)
        {
            IServiceScope scope = null;
            try
            {
                scope = serviceProvider.CreateScope();
                UpdateRefreshTokens(scope).Wait();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error while remove refresh tokens");
            }
            finally
            {
                scope?.Dispose();
            }
        }

        private async Task UpdateRefreshTokens(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var oldDate = DateTime.Now - options.Value.Expiration;
            var toDeleteRefreshTokens = await dbContext
                .RemovedTokens.Where(t => t.RemovedTime <= oldDate)
                .ToListAsync();
            logger.LogDebug($"Delete {toDeleteRefreshTokens.Count} refresh tokens");
            dbContext.RemoveRange(toDeleteRefreshTokens);
            await dbContext.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            logger.LogInformation("Started service");
            return Task.CompletedTask;
        }
    }
}
