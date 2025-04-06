
using FinTrack.API.Extensions;
using FinTrack.API.Services.Interfaces;

namespace FinTrack.API.BackgroundServices
{
    public class UpdateTransactionsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public UpdateTransactionsBackgroundService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var monobankService = scope.ServiceProvider.GetRequiredService<IMonobankService>();
            while (true)
            {
                var utcNow = DateTime.UtcNow;
                var utcNowTimestamp = utcNow.ToTimestampLong();
                var utc30DaysAgo = utcNow.AddDays(-30).ToTimestampLong();

                var clientInfo = await monobankService.GetClientInfoAsync();
                var accountIds = clientInfo.Accounts.Select(i => i.Id);

                foreach (var accountId in accountIds)
                {
                    await Task.Delay(62 * 1000);
                    await monobankService.RefreshTransactions(from: utc30DaysAgo, to: utcNowTimestamp, accountId);
                }
            }
        }
    }
}
