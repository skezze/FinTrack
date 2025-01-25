using System.Net.Http.Json;
using System.Transactions;

public class MonobankService : IBankService
{
    private readonly HttpClient _httpClient;

    public MonobankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SyncTransactionsAsync()
    {
        var response = await _httpClient.GetAsync("https://api.monobank.ua/personal/statement/...");
        response.EnsureSuccessStatusCode();

        var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();
        // Сохранение транзакций в базу
    }

    public async Task<IEnumerable<Account>> GetAccountsAsync()
    {
        var response = await _httpClient.GetAsync("https://api.monobank.ua/personal/accounts");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Account>>();
    }

    public async Task<decimal> GetBalanceAsync()
    {
        var response = await _httpClient.GetAsync("https://api.monobank.ua/personal/balance");
        response.EnsureSuccessStatusCode();

        var balance = await response.Content.ReadFromJsonAsync<decimal>();
        return balance;
    }
}
