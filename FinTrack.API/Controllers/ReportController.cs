using FinTrack.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IPdfReportGeneratorService pdfReportGeneratorService;
        private readonly ApplicationDbContext dbContext;

        public ReportController(
            IPdfReportGeneratorService pdfReportGeneratorService,
            ApplicationDbContext dbContext
            )
        {
            this.pdfReportGeneratorService = pdfReportGeneratorService;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GeneratePdfReportByUser(string accountId)
        {
            if(await dbContext.MonobankTransactions.AnyAsync(i => i.AccountId == accountId))
            {
                var filteredByAccountIdTransactions = await dbContext
                    .MonobankTransactions
                    .Where(i => i.AccountId == accountId)
                    .AsNoTracking()
                    .ToListAsync();

                var filePath = "D:/report.pdf";
                pdfReportGeneratorService.GenerateFinancialReport(filePath, filteredByAccountIdTransactions, accountId);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Файл не найден");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/pdf", $"financial_report_utc_{DateTime.UtcNow.ToString()}.pdf");
            }

            return NotFound("Транзакций по счету не найдено");
        }
    }
}
