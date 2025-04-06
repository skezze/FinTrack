using FinTrack.API.Entities;

namespace FinTrack.API.Services.Interfaces
{
    public interface IMonobankService
    {
        Task<MonobankClientInfo?> GetClientInfoAsync();
        Task RefreshTransactions(long from, long to, string accountId);
        Task AddTransactionIfNeeded(List<MonobankTransaction> transactions, string accountId);
    }
}
