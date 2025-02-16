using FinTrack.API.Entities;
using FinTrack.API.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FinTrack.API.Services
{
    public class MonobankService : IMonobankService
    {
        private string ApiKey { get; set; }
        private readonly HttpClient _httpClient;
        private string BaseUrl { get; set; }

        public MonobankService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            ApiKey = configuration["X-Token"]!;
            BaseUrl = configuration["BaseUrl"]!;
        }

        /// <summary>
        /// Получение информации о клиенте
        /// </summary>
        public async Task<MonobankClientInfo?> GetClientInfoAsync()
        {
            return await SendRequestAsync<MonobankClientInfo>($"{BaseUrl}/client-info");
        }

        /// <summary>
        /// Получение выписки по счету за промежуток времени
        /// </summary>
        public async Task<List<MonobankTransaction>?> GetStatementAsync(string accountId, long from, long to)
        {
            return await SendRequestAsync<List<MonobankTransaction>>($"{BaseUrl}/statement/{accountId}/{from}/{to}");
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
    }

}
