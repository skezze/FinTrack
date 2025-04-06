using FinTrack.API.Entities;

namespace FinTrack.API.Services.Interfaces
{
    public interface IPdfReportGeneratorService
    {
        Task<byte[]> GenerateReportBytesAsync(List<MonobankTransaction> transactions, string accountId, bool signDocument);
    }
}
