using FinTrack.API.Entities;

namespace FinTrack.API.Services.Interfaces
{
    public interface IPdfReportGeneratorService
    {
        void GenerateFinancialReport(string filePath, List<MonobankTransaction> transactions, string accountId);
    }
}
