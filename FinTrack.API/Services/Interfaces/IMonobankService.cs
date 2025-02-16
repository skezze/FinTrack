using FinTrack.API.Entities;

namespace FinTrack.API.Services.Interfaces
{
    public interface IMonobankService
    {
        Task<MonobankClientInfo?> GetClientInfoAsync();
        Task<List<MonobankTransaction>?> GetStatementAsync(string accountId, long from, long to);
    }
}
