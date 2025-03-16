using FinTrack.API.Entities;
using FinTrack.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FinTrack.API.Services
{
    public class MonobankService : IMonobankService
    {
        private string ApiKey { get; set; }
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl { get; set; }

        public MonobankService(
            HttpClient httpClient,
            IConfiguration configuration,
            ApplicationDbContext dbContext
            )
        {
            _httpClient = httpClient;
            this.dbContext = dbContext;
            ApiKey = configuration["MonoBank:X-Token"]!;
            BaseUrl = configuration["MonoBank:BaseUrl"]!;
        }

        /// <summary>
        /// Получение информации о клиенте
        /// </summary>
        public async Task<MonobankClientInfo?> GetClientInfoAsync()
        {
            return await SendRequestAsync<MonobankClientInfo>($"{BaseUrl}/client-info");
        }

        /// <summary>
        /// Получение выписки по счету за промежуток времени не больше 30 дней и 500 транзакций
        /// </summary>
        public async Task<List<MonobankTransaction>?> GetStatementAsync(long from, long to, string accountId = "0")//"0" is default account
        {
            var transactions = await SendRequestAsync<List<MonobankTransaction>>($"{BaseUrl}/statement/{accountId}/{from}/{to}");

            if (transactions is not null)
            {
                await AddTransactionIfNeeded(transactions!, accountId);
            }
            
            return transactions;
        }

        /// <summary>
        /// Универсальный метод для запросов
        /// </summary>
        private async Task<T?> SendRequestAsync<T>(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Token", ApiKey);

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка запроса: {ex.Message}");
                return default;
            }
        }

        public async Task AddTransactionIfNeeded(List<MonobankTransaction> transactions, string accountId)
        {
            var transactionIds = await dbContext
                .MonobankTransactions
                .Select(i => i.Id)
                .AsNoTracking()
                .ToListAsync();

            foreach(var transaction in transactions)
            {
                if(!transactionIds.Contains(transaction.Id))
                {
                    transaction.AccountId = accountId;
                    await dbContext.MonobankTransactions.AddAsync(transaction);
                }

                await dbContext.SaveChangesAsync();
            }

        }
    }

}
