using System.Net.Http.Json;
using System.Transactions;

public class PrivatBankService : IBankService
{
    private readonly HttpClient _httpClient;

    public PrivatBankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SyncTransactionsAsync()
    {
        var response = await _httpClient.GetAsync("https://api.privatbank.ua/p24api/...");
        response.EnsureSuccessStatusCode();

        var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();
        // Сохранение транзакций в базу
    }

    public async Task<IEnumerable<Account>> GetAccountsAsync()
    {
        var response = await _httpClient.GetAsync("https://api.privatbank.ua/p24api/accounts");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Account>>();
    }

    public async Task<decimal> GetBalanceAsync()
    {
        var response = await _httpClient.GetAsync("https://api.privatbank.ua/p24api/balance");
        response.EnsureSuccessStatusCode();

        var balance = await response.Content.ReadFromJsonAsync<decimal>();
        return balance;
    }
}
