using FinTrack.API.Entities;

namespace FinTrack.API.Services.Interfaces
{
    public interface IMonobankService
    {
        Task<MonobankClientInfo?> GetClientInfoAsync();
        Task<List<MonobankTransaction>?> GetStatementAsync(long from, long to, string accountId);
        Task AddTransactionIfNeeded(List<MonobankTransaction> transactions, string accountId);
    }
}
