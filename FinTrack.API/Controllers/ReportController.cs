using FinTrack.API.Entities;
using FinTrack.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Для IConfiguration
using System;
using System.ComponentModel.DataAnnotations;
using System.IO; // Для Path, Directory, File
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IPdfReportGeneratorService _pdfReportGeneratorService;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _reportStoragePath; // Путь к папке отчетов

        public ReportController(
            IPdfReportGeneratorService pdfReportGeneratorService,
            ApplicationDbContext dbContext,
            IConfiguration configuration // Добавляем IConfiguration
            )
        {
            this._pdfReportGeneratorService = pdfReportGeneratorService;
            this._dbContext = dbContext;

            // Получаем путь из конфигурации (appsettings.json) или используем путь по умолчанию
            _reportStoragePath = configuration.GetValue<string>("ReportSettings:StoragePath") ?? Path.Combine(AppContext.BaseDirectory, "Reports");

            // Убеждаемся, что папка существует при запуске приложения
            if (!Directory.Exists(_reportStoragePath))
            {
                try
                {
                    Directory.CreateDirectory(_reportStoragePath);
                }
                catch (Exception ex)
                {
                    // Логируем ошибку создания папки - возможно, нет прав
                    Console.WriteLine($"FATAL: Could not create report storage directory '{_reportStoragePath}'. Error: {ex.Message}");
                    // Можно пробросить исключение, чтобы приложение не стартовало без папки отчетов
                    // throw;
                }
            }
        }

        // Внутри ReportController.cs

        // Используем HttpPost, т.к. могут передаваться параметры для генерации
        [HttpPost("Generate")] // Меняем маршрут и метод на POST
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)] // Возвращаем файл
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

                // --- ДОБАВИТЬ ФИЛЬТРЫ ПО ДАТАМ (если нужно) ---
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
                // -----------------------------------------

                var transactions = await query.OrderBy(t => t.Time).AsNoTracking().ToListAsync();

                if (!transactions.Any())
                {
                    return NotFound($"Транзакцій по рахунку ID '{request.AccountId}' не знайдено за вказаний період.");
                }

                // Передаем параметр SignDocument
                byte[] pdfBytes = await _pdfReportGeneratorService.GenerateReportBytesAsync(transactions, request.AccountId, request.SignDocument);

                // 3. Проверяем результат от сервиса
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return StatusCode(500, "Помилка генерації звіту: сервіс повернув пустий результат.");
                }


                // 4. Формируем имя файла для скачивания
                var downloadFileName = $"FinReport_{request.AccountId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

                // 5. Возвращаем файл напрямую из байтов
                return File(pdfBytes, "application/pdf", downloadFileName);

                // --- Работа с файловой системой больше не нужна в этом методе! ---
                // --- Удаляем генерацию filePath, File.Exists, ReadAllBytesAsync и блок finally ---

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутрішня помилка сервера при генерації звіту.");
            }
            // Блок finally для удаления файла больше не нужен, т.к. мы не создаем файл на диске в этом методе
        }

        // Модель для запроса (остается как была предложена ранее)
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