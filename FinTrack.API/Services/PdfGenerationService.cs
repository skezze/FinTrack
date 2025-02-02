using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

public class PdfGenerationService
{
    private readonly IWebHostEnvironment _env;

    public PdfGenerationService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GenerateReportAsync(string userId, List<Transaction> transactions)
    {
        string reportsPath = Path.Combine(_env.WebRootPath, "reports");
        Directory.CreateDirectory(reportsPath); // Создаем папку, если ее нет

        string fileName = $"Report_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        string filePath = Path.Combine(reportsPath, fileName);

        using (var writer = new PdfWriter(filePath))
        using (var pdf = new PdfDocument(writer))
        {
            var document = new Document(pdf);
            document.Add(new Paragraph("Financial Report").SetFontSize(16));

            foreach (var transaction in transactions)
            {
                document.Add(new Paragraph($"{transaction.Date:yyyy-MM-dd} | {transaction.Category}: {transaction.Amount:C}"));
            }

            document.Close();
        }

        await Task.CompletedTask;
        return filePath; // Возвращаем путь к PDF
    }
}
