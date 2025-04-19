using System.Text;
using FinTrack.API.Entities;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;
using iText.Layout;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using FinTrack.API.Services.Interfaces;

namespace FinTrack.API.Services
{
    public class PdfReportGeneratorService : IPdfReportGeneratorService
    {
        private const string FONT_PATH = @"C:\Windows\Fonts\times.ttf";

        public async Task<byte[]> GenerateReportBytesAsync(List<MonobankTransaction> transactions, string accountId, bool signDocument)
        {
            if (transactions == null)
            {
                Console.WriteLine("Warning: Transaction list is null. Generating empty report.");
                transactions = new List<MonobankTransaction>();
            }

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    PdfFont regularFont = PdfFontFactory.CreateFont(FONT_PATH, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED); // Улучшаем встраивание шрифта

                    CultureInfo cultureInfo = new CultureInfo("uk-UA");

                    using (var writer = new PdfWriter(memoryStream))
                    using (var pdf = new PdfDocument(writer))
                    using (var document = new Document(pdf))
                    {
                        document.Add(new Paragraph("Фінансовий звіт")
                            .SetFont(regularFont).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(5));
                        document.Add(new Paragraph($"Рахунок: {accountId}")
                            .SetFont(regularFont).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(15));

                        Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 30, 18, 18, 14, 10 })).UseAllAvailableWidth();
                        table.AddHeaderCell(CreateHeaderCell("№", regularFont));
                        table.AddHeaderCell(CreateHeaderCell("Опис", regularFont));
                        table.AddHeaderCell(CreateHeaderCell("Сума", regularFont));
                        table.AddHeaderCell(CreateHeaderCell("Баланс", regularFont));
                        table.AddHeaderCell(CreateHeaderCell("Дата", regularFont));
                        table.AddHeaderCell(CreateHeaderCell("Коментар", regularFont));

                        int index = 1;
                        foreach (var transaction in transactions)
                        {
                            DeviceRgb amountColor = transaction.Amount < 0 ? new DeviceRgb(200, 0, 0) : new DeviceRgb(0, 128, 0);
                            string amountString = (transaction.Amount / 100.0).ToString("N2", cultureInfo) + " грн.";
                            string balanceString = (transaction.Balance / 100.0).ToString("N2", cultureInfo) + " грн.";
                            string dateString = DateTimeOffset.FromUnixTimeSeconds(transaction.Time).LocalDateTime.ToString("dd.MM.yyyy HH:mm");

                            table.AddCell(CreateDataCell(index.ToString(), regularFont, TextAlignment.CENTER));
                            table.AddCell(CreateDataCell(transaction.Description ?? "-", regularFont, TextAlignment.LEFT));
                            table.AddCell(CreateDataCell(amountString, regularFont, TextAlignment.RIGHT).SetFontColor(amountColor));
                            table.AddCell(CreateDataCell(balanceString, regularFont, TextAlignment.RIGHT));
                            table.AddCell(CreateDataCell(dateString, regularFont, TextAlignment.CENTER));
                            table.AddCell(CreateDataCell(transaction.Comment ?? "-", regularFont, TextAlignment.LEFT));
                            index++;
                        }
                        document.Add(table);

                        if (signDocument)
                        {
                            Console.WriteLine("Signing requested - Placeholder for PDF signing logic using iText7 and BouncyCastle.");
                        }

                        document.Add(new Paragraph($"Звіт згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .SetFont(regularFont).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER).SetMarginTop(20));
                    }

                    Console.WriteLine($"PDF звіт успішно згенеровано в пам'ять для рахунку {accountId}. Signed: {signDocument}");

                    return await Task.FromResult(memoryStream.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Сталася помилка при генерації PDF в пам'ять: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        private Cell CreateHeaderCell(string text, PdfFont font)
        {
            return new Cell()
                .Add(new Paragraph(text).SetFont(font).SetFontSize(10))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(4)
                .SetBackgroundColor(new DeviceRgb(230, 230, 230))
                .SetBorder(new SolidBorder(new DeviceRgb(150, 150, 150), 0.5f));
        }

        private Cell CreateDataCell(string text, PdfFont font, TextAlignment alignment = TextAlignment.LEFT)
        {
            return new Cell()
                .Add(new Paragraph(text).SetFont(font).SetFontSize(9))
                .SetTextAlignment(alignment)
                .SetPadding(4)
                .SetBorder(new SolidBorder(new DeviceRgb(200, 200, 200), 0.5f));
        }
    }
}