using FinTrack.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IPdfReportGeneratorService _pdfReportGeneratorService;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _reportStoragePath;

        public ReportController(
            IPdfReportGeneratorService pdfReportGeneratorService,
            ApplicationDbContext dbContext,
            IConfiguration configuration
            )
        {
            this._pdfReportGeneratorService = pdfReportGeneratorService;
            this._dbContext = dbContext;

            _reportStoragePath = configuration.GetValue<string>("ReportSettings:StoragePath") ?? Path.Combine(AppContext.BaseDirectory, "Reports");

            if (!Directory.Exists(_reportStoragePath))
            {
                try
                {
                    Directory.CreateDirectory(_reportStoragePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FATAL: Could not create report storage directory '{_reportStoragePath}'. Error: {ex.Message}");
                }
            }
        }

        [HttpPost("Generate")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneratePdfReportByUser([FromBody] ReportRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var query = _dbContext.MonobankTransactions
                    .Where(t => t.AccountId == request.AccountId);

                if (request.StartDate.HasValue)
                {
                    var startTimestamp = ((DateTimeOffset)request.StartDate.Value.Date).ToUnixTimeSeconds();
                    query = query.Where(t => t.Time >= startTimestamp);
                }
                if (request.EndDate.HasValue)
                {
                    var endTimestamp = ((DateTimeOffset)request.EndDate.Value.Date.AddDays(1).AddTicks(-1)).ToUnixTimeSeconds();
                    query = query.Where(t => t.Time <= endTimestamp);
                }

                var transactions = await query.OrderBy(t => t.Time).AsNoTracking().ToListAsync();

                if (!transactions.Any())
                {
                    return NotFound($"Транзакцій по рахунку ID '{request.AccountId}' не знайдено за вказаний період.");
                }

                byte[] pdfBytes = await _pdfReportGeneratorService.GenerateReportBytesAsync(transactions, request.AccountId, request.SignDocument);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return StatusCode(500, "Помилка генерації звіту: сервіс повернув пустий результат.");
                }


                var downloadFileName = $"FinReport_{request.AccountId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

                return File(pdfBytes, "application/pdf", downloadFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутрішня помилка сервера при генерації звіту.");
            }
        }

        public class ReportRequestModel
        {
            [Required]
            public string AccountId { get; set; } = string.Empty;
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool SignDocument { get; set; } = false;
            public string ReportType { get; set; } = "TransactionSummary";
        }
    }
}