using FinTrack.API.Entities;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;
using iText.Layout;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using FinTrack.API.Services.Interfaces;

namespace FinTrack.API.Services
{
    public class PdfReportGeneratorService : IPdfReportGeneratorService
    {
        public void GenerateFinancialReport(string filePath, List<MonobankTransaction> transactions, string accountId)
        {
            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                var regularFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                document.Add(new Paragraph("Financial Report")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Account: {accountId}")
                    .SetFont(regularFont)
                    .SetFontSize(12)
                    .SetMarginBottom(10));

                Table table = new Table(new float[] { 50, 120, 60, 60, 80, 80 }).UseAllAvailableWidth();

                table.AddHeaderCell(new Cell().Add(new Paragraph("No.").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Amount").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Balance").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Comment").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)));

                int index = 1;
                foreach (var transaction in transactions)
                {
                    table.AddCell(new Cell().Add(new Paragraph(index.ToString()).SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(transaction.Description ?? "-").SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((transaction.Amount / 100.0).ToString("F2", CultureInfo.InvariantCulture)).SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph((transaction.Balance / 100.0).ToString("F2", CultureInfo.InvariantCulture)).SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(DateTimeOffset.FromUnixTimeSeconds(transaction.Time).ToString("dd.MM.yyyy HH:mm")).SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    table.AddCell(new Cell().Add(new Paragraph(transaction.Comment ?? "-").SetFont(regularFont).SetTextAlignment(TextAlignment.CENTER)));
                    index++;
                }

                document.Add(table);
                document.Add(new Paragraph($"Report generated on: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .SetFont(regularFont)
                    .SetTextAlignment(TextAlignment.CENTER));
            }
        }
    }

}
