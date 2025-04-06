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
using System.IO; // Для MemoryStream
using System.Threading.Tasks; // Для Task

namespace FinTrack.API.Services
{
    public class PdfReportGeneratorService : IPdfReportGeneratorService
    {
        // TODO: Сделать путь к шрифту конфигурируемым или встроить шрифт в проект
        private const string FONT_PATH = @"C:\Windows\Fonts\times.ttf"; // Будьте осторожны с жестко заданными путями!


        // --- НОВЫЙ МЕТОД: Генерирует PDF в память и возвращает byte[] ---
        public async Task<byte[]> GenerateReportBytesAsync(List<MonobankTransaction> transactions, string accountId, bool signDocument)
        {
            if (transactions == null)
            {
                Console.WriteLine("Warning: Transaction list is null. Generating empty report.");
                transactions = new List<MonobankTransaction>();
            }

            // Используем MemoryStream для записи PDF в память
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    PdfFont regularFont = PdfFontFactory.CreateFont(FONT_PATH, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED); // Улучшаем встраивание шрифта

                    CultureInfo cultureInfo = new CultureInfo("uk-UA");

                    // Создаем PdfWriter для записи в MemoryStream
                    using (var writer = new PdfWriter(memoryStream))
                    // Важно: не закрываем memoryStream здесь, iText сам управляет writer'ом
                    using (var pdf = new PdfDocument(writer))
                    using (var document = new Document(pdf))
                    {
                        // --- Логика добавления контента в PDF (остается такой же) ---
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

                        // --- TODO: ЛОГИКА ПОДПИСИ PDF (PAdES) ---
                        if (signDocument)
                        {
                            Console.WriteLine("Signing requested - Placeholder for PDF signing logic using iText7 and BouncyCastle.");
                            // Здесь нужно будет:
                            // 1. Загрузить приватный ключ (например, из файла или хранилища).
                            // 2. Подготовить данные для подписи (хеш документа).
                            // 3. Использовать возможности iText7 (возможно, PdfSigner) и BouncyCastle для создания
                            //    и встраивания цифровой подписи (например, PAdES B-B или B-T) в PDF.
                            //    Это более сложный процесс, чем просто генерация внешнего .sig файла.
                            //    Потребуется работа с объектами PdfPKCS7, IExternalSignature, IExternalDigest и т.д.
                            //    Пример ищите в документации iText 7 "digital signatures".
                            //    ВАЖНО: Подпись обычно применяется ПОСЛЕ добавления всего контента,
                            //    но ДО закрытия документа/потока, часто с использованием PdfStamper или PdfSigner.
                            //    Возможно, структуру using придется немного изменить для подписи.
                        }
                        // ---------------------------------------

                        document.Add(new Paragraph($"Звіт згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .SetFont(regularFont).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER).SetMarginTop(20));

                        // Документ закроется автоматически при выходе из блока using
                    } // Document, PdfDocument, PdfWriter закрываются здесь

                    Console.WriteLine($"PDF звіт успішно згенеровано в пам'ять для рахунку {accountId}. Signed: {signDocument}");

                    // Возвращаем содержимое MemoryStream как массив байт
                    // Используем Task.FromResult, т.к. генерация iText синхронная
                    return await Task.FromResult(memoryStream.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Сталася помилка при генерації PDF в пам'ять: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    // Пробрасываем исключение или возвращаем пустой массив/null
                    // return Task.FromResult(Array.Empty<byte>());
                    throw; // Лучше пробросить, чтобы контроллер вернул 500
                }
            } // MemoryStream освобождается здесь
              // Не нужно явно делать await Task.Run, т.к. iText синхронный,
              // а контроллер уже будет вызван в потоке из пула ASP.NET Core.
              // Просто возвращаем Task с результатом.
        }


        // Вспомогательные методы остаются без изменений
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